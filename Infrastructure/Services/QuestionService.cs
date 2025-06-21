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


        public QuestionService(IQuestionRepository questionRepo, ITestSectionRepository testSectionRepository, IMCQOptionRepository mCQOptionRepository, HangulLearningSystemDbContext dbContext)
        {
            _questionRepo = questionRepo;
            _testSectionRepository = testSectionRepository;
            _mCQOptionRepository = mCQOptionRepository;
            _dbContext = dbContext;
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

        public async Task<OperationResult<bool>> UpdateQuestionAsync(UpdateQuestionCommand command)
        {
            var question = await _questionRepo.GetByIdAsync(command.QuestionID);
            if (question == null)
                return OperationResult<bool>.Fail("Không tìm thấy câu hỏi.");

            var validateContent = ValidateExactlyOneContent(command.Context, command.ImageURL, command.AudioURL);
            if (!validateContent.Success)
                return validateContent;

            question.Context = command.Context;
            question.ImageURL = command.ImageURL;
            question.AudioURL = command.AudioURL;

            // Nếu là dạng Writing, chỉ cập nhật Questions
            if (question.Type == TestFormatType.Writing)
            {
                await _questionRepo.UpdateAsync(question);
                return OperationResult<bool>.Ok(true, "Cập nhật câu hỏi dạng Writing thành công.");
            }

            // Với Multiple/TrueFalse phải có options
            if (command.Options == null || !command.Options.Any())
                return OperationResult<bool>.Fail("Câu hỏi dạng trắc nghiệm phải có ít nhất một đáp án.");

            var optionValidation = ValidateMCQOptions(command.Options);
            if (!optionValidation.Success)
                return optionValidation;

            // Mở transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                // Cập nhật câu hỏi chính
                await _questionRepo.UpdateAsync(question);

                // Xóa đáp án cũ
                await _mCQOptionRepository.DeleteByQuestionIdAsync(question.QuestionID);

                // Thêm đáp án mới
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

                // Commit transaction
                await transaction.CommitAsync();
                return OperationResult<bool>.Ok(true, "Cập nhật câu hỏi trắc nghiệm thành công.");
            }
            catch (Exception)
            {
                try
                {
                    if (_dbContext.Database.CurrentTransaction != null)
                        await transaction.RollbackAsync();
                }
                catch { }

                return OperationResult<bool>.Fail("Đã xảy ra lỗi khi cập nhật câu hỏi.");
            }
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



        public OperationResult<bool> ValidateMCQOptions(List<MCQOptionDto>? options)
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
        public async Task<OperationResult<List<TestSectionWithQuestionsDto>>> GetQuestionsByTestIdAsync(string testId)
        {
            var sections = await _testSectionRepository.GetByTestIdAsync(testId);
            if (sections == null || !sections.Any())
                return OperationResult<List<TestSectionWithQuestionsDto>>.Fail("Không tìm thấy phần thi nào.");

            var result = new List<TestSectionWithQuestionsDto>();

            foreach (var section in sections)
            {
                var questions = (await _questionRepo.GetQuestionBySectionId(section.TestSectionID))
                                .Where(q => q.IsActive).ToList();

                var questionDtos = new List<QuestionDetailDto>();

                foreach (var question in questions)
                {
                    var dto = new QuestionDetailDto
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
                        dto.Options = options.Select(o => new MCQOptionDto
                        {
                            Context = o.Context,
                            ImageURL = o.ImageURL,
                            AudioURL = o.AudioURL,
                            IsCorrect = o.IsCorrect
                        }).ToList();
                    }

                    questionDtos.Add(dto);
                }

                result.Add(new TestSectionWithQuestionsDto
                {
                    TestSectionID = section.TestSectionID,
                    Context = section.Context,
                    TestSectionType = section.TestSectionType,
                    Score = section.Score,
                    Questions = questionDtos
                });
            }

            return OperationResult<List<TestSectionWithQuestionsDto>>.Ok(result);
        }

    }

}
