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
namespace Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;

        public QuestionService(IQuestionRepository questionRepo)
        {
            _questionRepo = questionRepo;
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

            decimal scorePerQuestion;
            if (command.FormatType == TestFormatType.Writing)
            {
                scorePerQuestion = 100m;
            }
            else
            {
                if (requested <= 0)
                    return OperationResult<List<Question>>.Fail("Số lượng câu hỏi phải lớn hơn 0.");
                scorePerQuestion = (decimal)command.Score / requested;
            }

            int delta = requested - current;

            if (delta > 0)
            {
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

                var allActiveQuestions = existing.Where(q => q.IsActive).ToList();
                foreach (var q in allActiveQuestions.Where(q => q.Score != scorePerQuestion))
                {
                    q.Score = scorePerQuestion;
                }
                await _questionRepo.UpdateRangeAsync(allActiveQuestions.Where(q => !newQuestions.Contains(q)).ToList());
            }
            else if (delta < 0)
            {
                // Cần disable bớt câu hỏi
                var toDisable = active.Skip(requested).ToList();
                foreach (var q in toDisable)
                {
                    q.IsActive = false;
                    q.Score = 0;
                }
                await _questionRepo.UpdateRangeAsync(toDisable);

                // Cập nhật điểm cho các câu hỏi còn active
                var remainingActive = active.Take(requested).ToList();
                foreach (var q in remainingActive)
                {
                    q.Score = scorePerQuestion;
                }
                await _questionRepo.UpdateRangeAsync(remainingActive);
            }
            else
            {
                // Số lượng không thay đổi nhưng có thể cần cập nhật điểm
                foreach (var q in active)
                {
                    q.Score = scorePerQuestion;
                }
                await _questionRepo.UpdateRangeAsync(active);
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
