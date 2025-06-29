using Domain.Entities;
using Application.Common.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.IRepositories
{
    public interface ITestRepository
    {
        Task<OperationResult<string>> CreateTestAsync(Test test);
        Task<OperationResult<Test>> GetTestByIdAsync(string testId);
        Task<OperationResult<List<Test>>> GetTestsByAccountIdAsync(string accountId);
        Task<OperationResult<string>> UpdateTestAsync(Test test);
        Task<OperationResult<string>> DeleteTestAsync(string testId);
        Task<string> GenerateNextTestIdAsync();
        Task<List<Test>> GetPendingTestsOlderThanDaysAsync(int days);
        Task<OperationResult<string>> UpdateTestStatusAsync(string testId, Domain.Enums.TestStatus status);
        Task<OperationResult<List<Test>>> GetAllTestsAsync();
        Task<OperationResult<List<Test>>> GetAllTestsWithFiltersAsync(string? status = null, string? createdBy = null);
    }
}