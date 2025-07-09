using Application.Usecases.Command;
using Application.Common.Constants;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Enums;
using Application.Common.Shared;

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
        Task<OperationResult<List<Test>>> GetTestsWithAdvancedFiltersAsync(
            AssessmentCategory? category = null,
            string? subjectId = null,
            TestType? testType = null,
            TestStatus? status = null);
        Task<OperationResult<List<StudentTestResultDTO>>> GetListStudentTestResultsByTestEventAsync(string testEventId);
        Task<OperationResult<StudentTestResultDTO>> GetStudentTestResultsByTestEventAsync(string testEventId, string accountId);
        Task<OperationResult<PagedResult<TestDetailDTO>>> GetListAsync(int page, int pageSize);
        Task<OperationResult<PagedResult<TestDetailDTO>>> GetListByStatusAsync(string status, int page, int pageSize);
        Task<OperationResult<PagedResult<TestDetailDTO>>> GetListByCreatorAsync(string createdBy, int page, int pageSize);
        Task<OperationResult<PagedResult<TestDetailDTO>>> GetListBySubjectAsync(string subjectId, int page, int pageSize);
        Task<OperationResult<PagedResult<TestDetailDTO>>> GetListByTestTypeAsync(TestType testType, int page, int pageSize);
        Task<OperationResult<PagedResult<TestDetailDTO>>> GetListByCategoryAsync(AssessmentCategory category, int page, int pageSize);
        Task<OperationResult<PagedResult<TestDetailDTO>>> GetListWithFiltersAsync(
            string? status = null,
            string? createdBy = null,
            string? subjectId = null,
            TestType? testType = null,
            AssessmentCategory? category = null,
            int page = 1,
            int pageSize = 10);
        //Kho - làm tạm hàm update status mới
        Task<OperationResult<string>> UpdateTestStatusFixAsync(UpdateTestStatusFixCommand request);
        Task<OperationResult<StudentTestResultDTO>> GetStudentTestResultByStudentTestIDAsync(string studentTestID);

    }
}