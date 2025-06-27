using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Application.Common.Constants;
using Application.Usecases.Command;
using Application.DTOs;

namespace Application.IServices
{
    public interface IQuestionService
    {
        Task<OperationResult<List<Question>>> CreateEmptyQuestionsAsync(CreateQuestionsCommand command);
        Task<bool> IsTestFormatTypeConsistentAsync(string testSectionId, TestFormatType formatType);
        OperationResult<string> ValidateWritingQuestionRule(TestFormatType type, int numberOfQuestions);
        Task<OperationResult<bool>> UpdateQuestionAsync(UpdateQuestionCommand command);
        Task<OperationResult<bool>> ValidateQuestionExistsAsync(string questionId);
        OperationResult<bool> ValidateExactlyOneContent(string? context, string? imageUrl, string? audioUrl);
        OperationResult<bool> ValidateMCQOptions(List<MCQOptionDTO>? options);
        Task<Question?> GetByIdAsync(string questionId);
        Task<Question?> GetQuestionByIdAsync(string questionId);
        Task<OperationResult<List<TestSectionWithQuestionsDTO>>> GetQuestionsByTestIdAsync(string testId);
        Task<OperationResult<bool>> ValidateOptionCountLimitAsync(List<MCQOptionDTO> options);
        OperationResult<bool> ValidateMCQOptionsNoDuplicate(List<MCQOptionDTO>? options);
        Task<OperationResult<bool>> UpdateMultipleQuestionsAsync(List<UpdateQuestionCommand> commands);
        Task<OperationResult<List<Question>>> GetQuestionsByTestSectionIDAsync(string testSectionID);

    }
}
