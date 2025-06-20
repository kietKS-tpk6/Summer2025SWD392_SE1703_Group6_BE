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

            float scorePerQuestion = command.FormatType == TestFormatType.Writing
                ? 100f
                : command.Score / requested;

            int delta = requested - current;
            int nextIndex = await _questionRepo.GetTotalQuestionCount();

            if (delta > 0)
            {
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
            }
            else if (delta < 0)
            {
                var toDisable = active.Skip(requested).ToList();
                foreach (var q in toDisable)
                {
                    q.IsActive = false;
                    q.Score = 0;
                }

                await _questionRepo.UpdateRangeAsync(toDisable);
            }

            var updatedActive = existing.Where(q => q.IsActive).ToList();
            foreach (var q in updatedActive)
            {
                q.Score = scorePerQuestion;
            }

            await _questionRepo.UpdateRangeAsync(updatedActive);

            return OperationResult<List<Question>>.Ok(updatedActive, "Tạo câu hỏi thành công.");
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
