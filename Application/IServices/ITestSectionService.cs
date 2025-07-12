using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;

namespace Application.IServices
{
    public interface ITestSectionService
    {
        Task<bool> IsTestSectionExistAsync(string testSectionId);
        Task<OperationResult<string>> ValidateSectionTypeMatchFormatAsync(string testSectionId, TestFormatType formatType);
        Task<List<TestSection>> GetByTestIdAsync(string testId);

        //TA did it
        Task<OperationResult<string>> CreateTestSectionAsync(CreateTestSectionCommand command);
        Task<OperationResult<string>> UpdateTestSectionAsync(UpdateTestSectionCommand command);
        Task<OperationResult<string>> DeleteTestSectionAsync(DeleteTestSectionCommand command);
        Task<OperationResult<TestSection>> GetTestSectionByIdAsync(string testSectionId);
        Task<OperationResult<List<TestSection>>> GetTestSectionsByTestIdAsync(string testId);
        Task<string> GenerateNextTestSectionIdAsync();
    }
}
