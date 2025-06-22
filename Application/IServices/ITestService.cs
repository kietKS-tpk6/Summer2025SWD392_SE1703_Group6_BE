using Application.Usecases.Command;
using Application.Common.Constants;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ITestService
    {
        Task<OperationResult<string>> CreateTestAsync(CreateTestCommand command);
        Task<OperationResult<string>> UpdateTestAsync(UpdateTestCommand command);
        Task<OperationResult<string>> UpdateTestStatusAsync(UpdateTestStatusCommand command);
        Task<OperationResult<string>> DeleteTestAsync(DeleteTestCommand command);
        Task<OperationResult<Test>> GetTestByIdAsync(string testId);
        Task<OperationResult<List<Test>>> GetTestsByAccountIdAsync(string accountId);
        Task<string> GenerateNextTestIdAsync();
        Task ProcessAutoApprovalAsync();
        Task<OperationResult<List<Test>>> GetAllTestsAsync();
        Task<OperationResult<List<Test>>> GetAllTestsWithFiltersAsync(string? status = null, string? createdBy = null);
    }
}