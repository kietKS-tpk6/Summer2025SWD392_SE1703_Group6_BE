using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Infrastructure.IRepositories;
using Domain.Entities;
using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Enums;
using Infrastructure.Repositories;
using Application.DTOs;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
namespace Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly ITestSectionRepository _testSectionRepository;
        private readonly IMCQOptionRepository _mCQOptionRepository;
        private readonly HangulLearningSystemDbContext _dbContext;
        private readonly ISystemConfigService _systemConfigService;
        private readonly ITestRepository _testRepository;


        public QuestionService(IQuestionRepository questionRepo, ITestSectionRepository testSectionRepository, IMCQOptionRepository mCQOptionRepository, HangulLearningSystemDbContext dbContext, ISystemConfigService systemConfigService,ITestRepository testRepository)
        {
            _questionRepo = questionRepo;
            _testSectionRepository = testSectionRepository;
            _mCQOptionRepository = mCQOptionRepository;
            _dbContext = dbContext;
            _systemConfigService = systemConfigService;
            _testRepository = testRepository;
        }


        public OperationResult<string> ValidateWritingQuestionRule(TestFormatType type, int numberOfQuestions)
        {
            if (type == TestFormatType.Writing && numberOfQuestions != 1)
            {
                return OperationResult<string>.Fail("Dạng Writing chỉ được phép có 1 câu hỏi.");
            }

            return OperationResult<string>.Ok("Hợp lệ.");
        }

        public async Task<OperationResult<List<Question>>> CreateEmptyQuestionsAsync(CreateQuestionsCommand command)
        {
            var existing = (await _questionRepo.GetQuestionBySectionId(command.TestSectionID)).ToList();
            var active = existing.Where(q => q.IsActive).ToList();
            int current = active.Count;
            int requested = command.NumberOfQuestions;

            if (command.FormatType == TestFormatType.Writing && requested != 1)
                return OperationResult<List<Question>>.Fail("Dạng Writing chỉ được phép có 1 câu hỏi.");

            // Lấy điểm từ TestSection cho tất cả các format type
            var sectionScore = await _testSectionRepository.GetScoreByTestSectionIdAsync(command.TestSectionID);
            if (sectionScore == null || sectionScore.Value <= 0)
                return OperationResult<List<Question>>.Fail("Không tìm thấy điểm hợp lệ trong TestSection.");

            decimal scorePerQuestion;
            if (command.FormatType == TestFormatType.Writing)
            {
                // Writing: truyền nguyên điểm của section vào câu hỏi duy nhất
                scorePerQuestion = sectionScore.Value;
            }
            else
            {
                if (requested <= 0)
                    return OperationResult<List<Question>>.Fail("Số lượng câu hỏi phải lớn hơn 0.");

                // Các format khác: chia đều điểm cho các câu hỏi
                scorePerQuestion = Math.Round(sectionScore.Value / (decimal)requested, 2);
            }

            int delta = requested - current;

            if (delta > 0)
            {
                // Tạo câu hỏi mới
                int nextIndex = await _questionRepo.GetTotalQuestionCount();
                var newQuestions = Enumerable.Range(1, delta).Select(i => new Question
                {
                    QuestionID = "Q" + (nextIndex + i).ToString("D6"),
                    TestSectionID = command.TestSectionID,
                    Type = command.FormatType,
                    IsActive = true,
                    Score = scorePerQuestion,
                    Context = null,
                    ImageURL = null,
                    AudioURL = null
                }).ToList();

                await _questionRepo.AddRangeAsync(newQuestions);
                existing.AddRange(newQuestions);

                // Cập nhật điểm cho các câu hỏi cũ nếu khác
                var oldActiveQuestions = existing.Where(q => q.IsActive && !newQuestions.Contains(q)).ToList();
                var questionsToUpdate = oldActiveQuestions.Where(q => q.Score != scorePerQuestion).ToList();

                if (questionsToUpdate.Any())
                {
                    foreach (var q in questionsToUpdate)
                    {
                        q.Score = scorePerQuestion;
                    }
                    await _questionRepo.UpdateRangeAsync(questionsToUpdate);
                }
            }
            else if (delta < 0)
            {
                // Disable các câu hỏi thừa
                var toDisable = active.Skip(requested).ToList();
                foreach (var q in toDisable)
                {
                    q.IsActive = false;
                    q.Score = 0;
                }
                await _questionRepo.UpdateRangeAsync(toDisable);

                // Cập nhật điểm cho các câu hỏi còn lại
                var remainingActive = active.Take(requested).ToList();
                var questionsToUpdate = remainingActive.Where(q => q.Score != scorePerQuestion).ToList();

                if (questionsToUpdate.Any())
                {
                    foreach (var q in questionsToUpdate)
                    {
                        q.Score = scorePerQuestion;
                    }
                    await _questionRepo.UpdateRangeAsync(questionsToUpdate);
                }
            }
            else
            {
                // Số lượng không đổi, chỉ cập nhật điểm nếu cần
                var questionsToUpdate = active.Where(q => q.Score != scorePerQuestion).ToList();

                if (questionsToUpdate.Any())
                {
                    foreach (var q in questionsToUpdate)
                    {
                        q.Score = scorePerQuestion;
                    }
                    await _questionRepo.UpdateRangeAsync(questionsToUpdate);
                }
            }

            var finalActiveQuestions = existing.Where(q => q.IsActive).ToList();
            return OperationResult<List<Question>>.Ok(finalActiveQuestions, "Tạo/cập nhật câu hỏi thành công.");
        }
        
          public async Task<bool> IsTestFormatTypeConsistentAsync(string testSectionId, TestFormatType formatType)
        {
            var questions = await _questionRepo.GetQuestionBySectionId(testSectionId);
            var activeQuestions = questions.Where(q => q.IsActive).ToList();

            if (!activeQuestions.Any())
                return true;

            return activeQuestions.All(q => q.Type == formatType);
        }
        public async Task<OperationResult<bool>> UpdateMultipleQuestionsAsync(List<UpdateQuestionCommand> commands)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var command in commands)
                {
                    var result = await UpdateQuestionAsync(command);
                    if (!result.Success)
                    {
                        await transaction.RollbackAsync();
                        return OperationResult<bool>.Fail($"Cập nhật thất bại ở câu {command.QuestionID}: {result.Message}");
                    }
                }

                await transaction.CommitAsync();
                return OperationResult<bool>.Ok(true, "Cập nhật hàng loạt câu hỏi thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return OperationResult<bool>.Fail("Lỗi hệ thống: " + ex.Message);
            }
        }
        public async Task<OperationResult<bool>> UpdateQuestionAsync(UpdateQuestionCommand command)
        {
            var question = await _questionRepo.GetByIdAsync(command.QuestionID);
            if (question == null)
                return OperationResult<bool>.Fail("Không tìm thấy câu hỏi.");

            // Kiểm tra nội dung hợp lệ
            var validateContent = ValidateExactlyOneContent(command.Context, command.ImageURL, command.AudioURL);
            if (!validateContent.Success)
                return validateContent;

            question.Context = command.Context;
            question.ImageURL = command.ImageURL;
            question.AudioURL = command.AudioURL;

            if (question.Type == TestFormatType.Writing)
            {
                await _questionRepo.UpdateAsync(question);
                return OperationResult<bool>.Ok(true, "Cập nhật câu hỏi dạng Writing thành công.");
            }

            // Validate options
            if (command.Options == null || !command.Options.Any())
                return OperationResult<bool>.Fail("Câu hỏi dạng trắc nghiệm phải có ít nhất một đáp án.");
            if (!command.Options.Any(opt => opt.IsCorrect))
                return OperationResult<bool>.Fail("Phải có ít nhất một đáp án đúng (IsCorrect = true).");

            var optionValidation = ValidateMCQOptions(command.Options);
            if (!optionValidation.Success)
                return optionValidation;

            // Cập nhật câu hỏi chính
            await _questionRepo.UpdateAsync(question);

            // Xóa và thêm lại MCQ options
            await _mCQOptionRepository.DeleteByQuestionIdAsync(question.QuestionID);

            var newOptions = command.Options.Select(opt => new MCQOption
            {
                MCQOptionID = "MO" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                QuestionID = question.QuestionID,
                Context = opt.Context,
                ImageURL = opt.ImageURL,
                AudioURL = opt.AudioURL,
                IsCorrect = opt.IsCorrect
            }).ToList();

            await _mCQOptionRepository.AddRangeAsync(newOptions);

            return OperationResult<bool>.Ok(true, "Cập nhật câu hỏi trắc nghiệm thành công.");
        }


        public async Task<OperationResult<bool>> ValidateQuestionExistsAsync(string questionId)
        {
            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                return OperationResult<bool>.Fail("Không tìm thấy câu hỏi.");

            return OperationResult<bool>.Ok(true);
        }

        public OperationResult<bool> ValidateExactlyOneContent(string? context, string? imageUrl, string? audioUrl)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(context)) count++;
            if (!string.IsNullOrEmpty(imageUrl)) count++;
            if (!string.IsNullOrEmpty(audioUrl)) count++;

            if (count != 1)
                return OperationResult<bool>.Fail("Phải có đúng một trong Context, ImageURL hoặc AudioURL.");

            return OperationResult<bool>.Ok(true);
        }


        //Kho - tạm đổi sang DTO MCQOptionUpdateQuestionDTO (không có ID)
        public OperationResult<bool> ValidateMCQOptions(List<MCQOptionUpdateQuestionDTO>? options)
        {
            if (options == null || !options.Any())
                return OperationResult<bool>.Fail("Danh sách đáp án không được rỗng.");

            // Kiểm tra loại nội dung của đáp án đầu tiên
            string contentType = null;

            foreach (var option in options)
            {
                int nonNullCount = 0;
                string currentType = null;

                if (!string.IsNullOrWhiteSpace(option.Context))
                {
                    nonNullCount++;
                    currentType = "Context";
                }

                if (!string.IsNullOrWhiteSpace(option.ImageURL))
                {
                    nonNullCount++;
                    currentType = "ImageURL";
                }

                if (!string.IsNullOrWhiteSpace(option.AudioURL))
                {
                    nonNullCount++;
                    currentType = "AudioURL";
                }

                if (nonNullCount != 1)
                {
                    return OperationResult<bool>.Fail("Mỗi đáp án chỉ được phép có duy nhất một trong Context, ImageURL hoặc AudioURL.");
                }

                if (contentType == null)
                {
                    contentType = currentType;
                }
                else if (contentType != currentType)
                {
                    return OperationResult<bool>.Fail("Tất cả đáp án phải có cùng một loại nội dung (Context, ImageURL hoặc AudioURL).");
                }
            }

            if (!options.Any(o => o.IsCorrect))
                return OperationResult<bool>.Fail("Phải có ít nhất một đáp án đúng.");

            return OperationResult<bool>.Ok(true, "Danh sách đáp án hợp lệ.");
        }

        public async Task<Question?> GetByIdAsync(string questionId)
        {
            return await _questionRepo.GetByIdAsync(questionId);
        }

        public async Task<Question?> GetQuestionByIdAsync(string questionId)
        {
            return await _questionRepo.GetByIdAsync(questionId);
        }
        public async Task<OperationResult<List<TestSectionWithQuestionsDTO>>> GetQuestionsByTestIdAsync(string testId)
        {
            var sections = await _testSectionRepository.GetByTestIdAsync(testId);
            if (sections == null || !sections.Any())
                return OperationResult<List<TestSectionWithQuestionsDTO>>.Fail("Không tìm thấy phần thi nào.");

            var result = new List<TestSectionWithQuestionsDTO>();

            foreach (var section in sections)
            {
                var questions = (await _questionRepo.GetQuestionBySectionId(section.TestSectionID))
                                .Where(q => q.IsActive).ToList();

                var questionDTO = new List<QuestionDetailDTO>();

                foreach (var question in questions)
                {
                    var dto = new QuestionDetailDTO
                    {
                        QuestionID = question.QuestionID,
                        Context = question.Context,
                        ImageURL = question.ImageURL,
                        AudioURL = question.AudioURL,
                        Type = question.Type ?? TestFormatType.Writing,
                        Score = question.Score,
                        IsActive = question.IsActive,
                        Options = null
                    };

                    if (question.Type == TestFormatType.Multiple || question.Type == TestFormatType.TrueFalse)
                    {
                        var options = await _mCQOptionRepository.GetByQuestionIdAsync(question.QuestionID);
                        dto.Options = options.Select(o => new MCQOptionDTO
                        {
                            MCQOptionID = o.MCQOptionID,
                            Context = o.Context,
                            ImageURL = o.ImageURL,
                            AudioURL = o.AudioURL,
                            IsCorrect = o.IsCorrect
                        }).ToList();
                    }

                    questionDTO.Add(dto);
                }

                result.Add(new TestSectionWithQuestionsDTO
                {
                    TestSectionID = section.TestSectionID,
                    Context = section.Context,
                    TestSectionType = section.TestSectionType,
                    Score = section.Score,
                    Questions = questionDTO
                });
            }

            return OperationResult<List<TestSectionWithQuestionsDTO>>.Ok(result);
        }
        //Kho - Tạm đổi từ MCQOptionDTO sang MCQOptionUpdateQuestionDTO (không có MCQOptionID)
        public async Task<OperationResult<bool>> ValidateOptionCountLimitAsync(List<MCQOptionUpdateQuestionDTO> options)
        {
            var configResult = await _systemConfigService.GetConfig("max_mcq_option_per_question");

            if (!configResult.Success || configResult.Data == null)
            {
                return OperationResult<bool>.Fail("Không thể lấy cấu hình số lượng đáp án.");
            }

            if (!int.TryParse(configResult.Data.Value, out int maxCount))
            {
                return OperationResult<bool>.Fail("Giá trị cấu hình không hợp lệ.");
            }

            if (options.Count > maxCount)
            {
                return OperationResult<bool>.Fail($"Số lượng đáp án vượt quá giới hạn ({maxCount}).");
            }

            return OperationResult<bool>.Ok(true);
        }
        //Kho - Tạm đổi từ MCQOptionDTO sang MCQOptionUpdateQuestionDTO
        public OperationResult<bool> ValidateMCQOptionsNoDuplicate(List<MCQOptionUpdateQuestionDTO>? options)
        {
            if (options == null || !options.Any())
                return OperationResult<bool>.Fail("Danh sách đáp án không được rỗng.");

            var uniqueSet = new HashSet<string>();

            foreach (var option in options)
            {
                string key = $"{option.Context?.Trim()}|{option.ImageURL?.Trim()}|{option.AudioURL?.Trim()}".ToLower();
                if (!uniqueSet.Add(key))
                {
                    return OperationResult<bool>.Fail("Các đáp án không được trùng nhau.");
                }
            }

            return OperationResult<bool>.Ok(true, "Không có đáp án trùng nhau.");
        }

        public async Task<OperationResult<List<Question>>> GetQuestionsByTestSectionIDAsync(string testSectionID)
        {
            //if (string.IsNullOrWhiteSpace(testSectionID))
            //    return OperationResult<List<Question>>.Fail("TestSectionID không hợp lệ.");

            var questions = await _questionRepo.GetQuestionBySectionId(testSectionID);
            var activeQuestions = questions.Where(q => q.IsActive).ToList();

            return OperationResult<List<Question>>.Ok(activeQuestions);
        }

        public async Task<OperationResult<List<WritingQuestionWithBaremsDTO>>> GetWritingQuestionsWithBaremsByTestIDAsync(string testID)
        {
            var writingSections = await _testSectionRepository.GetByTestIDAndTypeAsync(testID, TestFormatType.Writing);

            var writingSectionIDs = writingSections.Select(s => s.TestSectionID).ToList();

            if (!writingSectionIDs.Any())
                return OperationResult<List<WritingQuestionWithBaremsDTO>>.Ok(new());

            var questions = await _questionRepo.GetByTestSectionIDsAsync(writingSectionIDs);

            var result = new List<WritingQuestionWithBaremsDTO>();

            foreach (var question in questions)
            {
                var barems = await _dbContext.WritingBarem
                    .Where(b => b.QuestionID == question.QuestionID)
                    .ToListAsync();

                result.Add(new WritingQuestionWithBaremsDTO
                {
                    QuestionID = question.QuestionID,
                    Context = question.Context,
                    ImageURL = question.ImageURL,
                    AudioURL = question.AudioURL,
                    Score = question.Score,
                    Barems = barems.Select(b => new WritingBaremDTO
                    {
                        WritingBaremID = b.WritingBaremID,
                        CriteriaName = b.CriteriaName,
                        MaxScore = b.MaxScore,
                        Description = b.Description
                    }).ToList()
                });
            }

            return OperationResult<List<WritingQuestionWithBaremsDTO>>.Ok(result);
        }
        public async Task<OperationResult<bool>> DeleteMCQOptionAsync(string questionID, string mcqOptionID)
        {
            var question = await _questionRepo.GetByIdAsync(questionID);
            if (question == null)
                return OperationResult<bool>.Fail("Không tìm thấy câu hỏi.");

            // Lấy TestSection
            var sectionResult = await _testSectionRepository.GetTestSectionByIdAsync(question.TestSectionID);
            if (!sectionResult.Success || sectionResult.Data == null)
                return OperationResult<bool>.Fail("Không tìm thấy phần thi chứa câu hỏi.");

            var testID = sectionResult.Data.TestID;

            // Lấy Test
            var testResult = await _testRepository.GetTestByIdAsync(testID);
            if (!testResult.Success || testResult.Data == null)
                return OperationResult<bool>.Fail("Không tìm thấy bài kiểm tra chứa phần thi.");

            if (testResult.Data.Status != TestStatus.Drafted)
                return OperationResult<bool>.Fail("Chỉ có thể xóa đáp án khi bài kiểm tra đang ở trạng thái 'Drafted'.");

            var options = await _mCQOptionRepository.GetByQuestionIdAsync(questionID);
            if (options == null || !options.Any())
                return OperationResult<bool>.Fail("Không tìm thấy danh sách đáp án.");

            var toDelete = options.FirstOrDefault(o => o.MCQOptionID == mcqOptionID);
            if (toDelete == null)
                return OperationResult<bool>.Fail("Không tìm thấy đáp án cần xóa.");

            // Kiểm tra nếu đang xóa đáp án đúng cuối cùng thì từ chối
            var remainingCorrectOptions = options.Count(o => o.IsCorrect && o.MCQOptionID != mcqOptionID);
            if (toDelete.IsCorrect && remainingCorrectOptions == 0)
                return OperationResult<bool>.Fail("Phải có ít nhất một đáp án đúng sau khi xóa.");

            await _mCQOptionRepository.DeleteAsync(toDelete);
            return OperationResult<bool>.Ok(true, "Xóa đáp án thành công.");
        }

        public async Task<OperationResult<bool>> SoftDeleteQuestionAsync(string questionId)
        {
            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                return OperationResult<bool>.Fail("Không tìm thấy câu hỏi.");

            if (!question.IsActive)
                return OperationResult<bool>.Fail("Câu hỏi đã bị vô hiệu hóa trước đó.");

            // Lấy section của câu hỏi
            var sectionResult = await _testSectionRepository.GetTestSectionByIdAsync(question.TestSectionID);
            if (!sectionResult.Success || sectionResult.Data == null)
                return OperationResult<bool>.Fail("Không tìm thấy phần thi chứa câu hỏi.");

            var testID = sectionResult.Data.TestID;

            // Gọi repository để lấy test
            var testResult = await _testRepository.GetTestByIdAsync(testID);
            if (!testResult.Success || testResult.Data == null)
                return OperationResult<bool>.Fail("Không tìm thấy bài kiểm tra chứa phần thi.");

            if (testResult.Data.Status != TestStatus.Drafted)
                return OperationResult<bool>.Fail("Chỉ có thể xóa câu hỏi khi bài kiểm tra đang ở trạng thái 'Drafted'.");

            // Kiểm tra còn lại ít nhất 1 câu active sau khi xóa
            var questionsInSection = await _questionRepo.GetQuestionBySectionId(sectionResult.Data.TestSectionID);
            var activeQuestions = questionsInSection.Where(q => q.IsActive && q.QuestionID != questionId).ToList();

            if (!activeQuestions.Any())
                return OperationResult<bool>.Fail("Không thể xóa. Mỗi phần thi phải có ít nhất 1 câu hỏi đang hoạt động.");

            question.IsActive = false;
            question.Score = 0;

            await _questionRepo.UpdateAsync(question);
            return OperationResult<bool>.Ok(true, "Xóa mềm câu hỏi thành công.");
        }

    }

}
