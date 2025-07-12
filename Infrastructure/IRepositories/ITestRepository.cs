using Domain.Entities;
using Application.Common.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Enums;
using Application.DTOs;

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
        Task<OperationResult<List<Test>>> GetTestsWithAdvancedFiltersAsync(
            AssessmentCategory? category = null,
            string? subjectId = null,
            TestType? testType = null,
            TestStatus? status = null);
        Task<OperationResult<(List<TestDetailDTO> Items, int TotalCount)>> GetPaginatedListAsync(int page, int pageSize);

        Task<OperationResult<(List<TestDetailDTO> Items, int TotalCount)>> GetPaginatedListByStatusAsync(string status, int page, int pageSize);

        Task<OperationResult<(List<TestDetailDTO> Items, int TotalCount)>> GetPaginatedListByCreatorAsync(string createdBy, int page, int pageSize);

        Task<OperationResult<(List<TestDetailDTO> Items, int TotalCount)>> GetPaginatedListBySubjectAsync(string subjectId, int page, int pageSize);

        Task<OperationResult<(List<TestDetailDTO> Items, int TotalCount)>> GetPaginatedListByTestTypeAsync(TestType testType, int page, int pageSize);

        Task<OperationResult<(List<TestDetailDTO> Items, int TotalCount)>> GetPaginatedListByCategoryAsync(AssessmentCategory category, int page, int pageSize);

        Task<OperationResult<(List<TestDetailDTO> Items, int TotalCount)>> GetPaginatedListWithFiltersAsync(
            string? status = null,
            string? createdBy = null,
            string? subjectId = null,
            TestType? testType = null,
            AssessmentCategory? category = null,
            int page = 1,
            int pageSize = 10);

        // kit {Lấy danh sách bài Test theo list TestID}
        Task<List<Test>> GetByIdsAsync(List<string> testIDs);
        //kit {Truy vấn Test theo TestID để lấy Category và TestName}
        Task<Test?> GetByIdAsync(string testID);
    }
}