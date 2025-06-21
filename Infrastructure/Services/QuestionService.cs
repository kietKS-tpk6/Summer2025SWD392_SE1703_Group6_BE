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
namespace Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly ITestSectionRepository _testSectionRepository;

        public QuestionService(IQuestionRepository questionRepo, ITestSectionRepository testSectionRepository)
        {
            _questionRepo = questionRepo;
            _testSectionRepository = testSectionRepository;
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
    }

}
