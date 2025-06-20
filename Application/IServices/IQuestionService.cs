using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Application.Common.Constants;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IQuestionService
    {
        Task<OperationResult<List<Question>>> CreateEmptyQuestionsAsync(CreateQuestionsCommand command);
        Task<bool> IsTestFormatTypeConsistentAsync(string testSectionId, TestFormatType formatType);
        OperationResult<string> ValidateWritingQuestionRule(TestFormatType type, int numberOfQuestions);

    }
}
