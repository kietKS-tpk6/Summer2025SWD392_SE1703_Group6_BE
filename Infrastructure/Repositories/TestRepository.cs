using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Enums;
using System.Linq.Expressions;
using Application.DTOs;

namespace Infrastructure.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public TestRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<string>> CreateTestAsync(Test test)
        {
            try
            {
                _dbContext.Test.Add(test);
                await _dbContext.SaveChangesAsync();
                return OperationResult<string>.Ok(test.TestID, OperationMessages.CreateSuccess("Test"));
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error creating test: {ex.Message}");
            }
        }

        public async Task<OperationResult<Test>> GetTestByIdAsync(string testId)
        {
            try
            {
                var test = await _dbContext.Test
                    .Include(t => t.Account)
                    .Include(t => t.Subject)
                    .FirstOrDefaultAsync(t => t.TestID == testId && t.Status != TestStatus.Deleted);

                if (test == null)
                    return OperationResult<Test>.Fail(OperationMessages.NotFound("Test"));

                return OperationResult<Test>.Ok(test);
            }
            catch (Exception ex)
            {
                return OperationResult<Test>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<List<Test>>> GetTestsByAccountIdAsync(string accountId)
        {
            try
            {
                var tests = await _dbContext.Test
                    .Include(t => t.Subject)
                    .Where(t => t.CreateBy == accountId && t.Status != TestStatus.Deleted)
                    .OrderByDescending(t => t.CreateAt)
                    .ToListAsync();

                return OperationResult<List<Test>>.Ok(tests);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Test>>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<string>> UpdateTestAsync(Test test)
        {
            try
            {
                test.UpdateAt = DateTime.Now;
                _dbContext.Test.Update(test);
                await _dbContext.SaveChangesAsync();
                return OperationResult<string>.Ok("", OperationMessages.UpdateSuccess("Test"));
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error updating test: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> DeleteTestAsync(string testId)
        {
            try
            {
                var test = await _dbContext.Test.FindAsync(testId);
                if (test == null)
                    return OperationResult<string>.Fail(OperationMessages.NotFound("Test"));

                test.Status = TestStatus.Deleted;
                test.UpdateAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                return OperationResult<string>.Ok("", OperationMessages.DeleteSuccess("Test"));
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error deleting test: {ex.Message}");
            }
        }

        public async Task<string> GenerateNextTestIdAsync()
        {
            var count = await _dbContext.Test.CountAsync();
            return $"T{(count + 1):D5}";
        }

        public async Task<List<Test>> GetPendingTestsOlderThanDaysAsync(int days)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            return await _dbContext.Test
                .Where(t => t.Status == TestStatus.Pending && t.CreateAt <= cutoffDate)
                .ToListAsync();
        }

        public async Task<OperationResult<string>> UpdateTestStatusAsync(string testId, TestStatus status)
        {
            try
            {
                var test = await _dbContext.Test.FindAsync(testId);
                if (test == null)
                    return OperationResult<string>.Fail(OperationMessages.NotFound("Test"));

                test.Status = status;
                test.UpdateAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                return OperationResult<string>.Ok("", OperationMessages.UpdateSuccess("Test Status"));
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error updating test status: {ex.Message}");
            }
        }
        public async Task<OperationResult<List<Test>>> GetAllTestsAsync()
        {
            try
            {
                var tests = await _dbContext.Test
                    .Include(t => t.Account)
                    .Include(t => t.Subject)
                    .Where(t => t.Status != TestStatus.Deleted)
                    .OrderByDescending(t => t.CreateAt)
                    .ToListAsync();

                return OperationResult<List<Test>>.Ok(tests);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Test>>.Fail($"Error retrieving tests: {ex.Message}");
            }
        }
        public async Task<OperationResult<List<Test>>> GetAllTestsWithFiltersAsync(string? status = null, string? createdBy = null)
        {
            try
            {
                var query = _dbContext.Test
                    .Include(t => t.Account)
                    .Include(t => t.Subject)
                    .Where(t => t.Status != TestStatus.Deleted);

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<TestStatus>(status, true, out var testStatus))
                {
                    query = query.Where(t => t.Status == testStatus);
                }

                if (!string.IsNullOrEmpty(createdBy))
                {
                    query = query.Where(t => t.CreateBy == createdBy);
                }

                var tests = await query
                    .OrderByDescending(t => t.CreateAt)
                    .ToListAsync();

                return OperationResult<List<Test>>.Ok(tests);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Test>>.Fail($"Error retrieving tests with filters: {ex.Message}");
            }
        }
        public async Task<OperationResult<List<Test>>> GetTestsWithAdvancedFiltersAsync(
    AssessmentCategory? category = null,
    string? subjectId = null,
    TestType? testType = null,
    TestStatus? status = null)
        {
            try
            {
                var query = _dbContext.Test
                    .Include(t => t.Account)
                    .Include(t => t.Subject)
                    .Where(t => t.Status != TestStatus.Deleted);

                if (category.HasValue)
                {
                    query = query.Where(t => t.Category == category.Value);
                }

                if (!string.IsNullOrEmpty(subjectId))
                {
                    query = query.Where(t => t.SubjectID == subjectId);
                }

                if (testType.HasValue)
                {
                    query = query.Where(t => t.TestType == testType.Value);
                }

                if (status.HasValue)
                {
                    query = query.Where(t => t.Status == status.Value);
                }

                var tests = await query
                    .OrderByDescending(t => t.CreateAt)
                    .ToListAsync();

                return OperationResult<List<Test>>.Ok(tests);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Test>>.Fail($"Error retrieving tests with advanced filters: {ex.Message}");
            }
        }
        public async Task<List<Test>> GetByIdsAsync(List<string> testIDs)
        {
            return await _dbContext.Test
                .Where(t => testIDs.Contains(t.TestID))
                .ToListAsync();
        }
        public async Task<Test?> GetByIdAsync(string testID)
        {
            return await _dbContext.Test.FirstOrDefaultAsync(t => t.TestID == testID);
        }
        private async Task<OperationResult<(List<TestDetailDTO> Items, int TotalCount)>> GetPaginatedTestListAsync(
        Expression<Func<Test, bool>> predicate, int page, int pageSize)
        {
            var query = _dbContext.Test
                .Include(t => t.Account)
                .Include(t => t.Subject)
                .Where(predicate);

            var totalCount = await query.CountAsync();

            var pagedTests = await query
                .OrderByDescending(t => t.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = pagedTests.Select(t => new TestDetailDTO
            {
                TestID = t.TestID,
                CreateBy = t.CreateBy,
                SubjectID = t.SubjectID,
                CreateAt = t.CreateAt,
                UpdateAt = (DateTime)t.UpdateAt,
                Status = t.Status,
                Category = t.Category,
                TestType = t.TestType,
                CreatedByName = t.Account?.FirstName,
                SubjectName = t.Subject?.SubjectName
            }).ToList();

            return OperationResult<(List<TestDetailDTO>, int)>.Ok((items, totalCount));
        }

        public Task<OperationResult<(List<TestDetailDTO>, int)>> GetPaginatedListAsync(int page, int pageSize)
        {
            return GetPaginatedTestListAsync(t => t.Status != TestStatus.Deleted, page, pageSize);
        }

        public Task<OperationResult<(List<TestDetailDTO>, int)>> GetPaginatedListByStatusAsync(string status, int page, int pageSize)
        {
            if (!Enum.TryParse<TestStatus>(status, out var parsedStatus))
            {
                return Task.FromResult(OperationResult<(List<TestDetailDTO>, int)>
                    .Fail(OperationMessages.InvalidInput("Trạng thái")));
            }

            return GetPaginatedTestListAsync(t => t.Status == parsedStatus, page, pageSize);
        }

        public Task<OperationResult<(List<TestDetailDTO>, int)>> GetPaginatedListByCreatorAsync(string createdBy, int page, int pageSize)
        {
            return GetPaginatedTestListAsync(t => t.Status != TestStatus.Deleted && t.CreateBy == createdBy, page, pageSize);
        }

        public Task<OperationResult<(List<TestDetailDTO>, int)>> GetPaginatedListBySubjectAsync(string subjectId, int page, int pageSize)
        {
            return GetPaginatedTestListAsync(t => t.Status != TestStatus.Deleted && t.SubjectID == subjectId, page, pageSize);
        }

        public Task<OperationResult<(List<TestDetailDTO>, int)>> GetPaginatedListByTestTypeAsync(TestType testType, int page, int pageSize)
        {
            return GetPaginatedTestListAsync(t => t.Status != TestStatus.Deleted && t.TestType == testType, page, pageSize);
        }

        public Task<OperationResult<(List<TestDetailDTO>, int)>> GetPaginatedListByCategoryAsync(AssessmentCategory category, int page, int pageSize)
        {
            return GetPaginatedTestListAsync(t => t.Status != TestStatus.Deleted && t.Category == category, page, pageSize);
        }

        public async Task<OperationResult<(List<TestDetailDTO>, int)>> GetPaginatedListWithFiltersAsync(
             string? status = null,
             string? createdBy = null,
             string? subjectId = null,
             TestType? testType = null,
             AssessmentCategory? category = null,
             int page = 1,
             int pageSize = 10)
        {
            try
            {
                var query = _dbContext.Test
                    .Include(t => t.Account)
                    .Include(t => t.Subject)
                    .Where(t => t.Status != TestStatus.Deleted);

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<TestStatus>(status, out var parsedStatus))
                {
                    query = query.Where(t => t.Status == parsedStatus);
                }

                if (!string.IsNullOrEmpty(createdBy))
                {
                    query = query.Where(t => t.CreateBy == createdBy);
                }

                if (!string.IsNullOrEmpty(subjectId))
                {
                    query = query.Where(t => t.SubjectID == subjectId);
                }

                if (testType.HasValue)
                {
                    query = query.Where(t => t.TestType == testType.Value);
                }

                if (category.HasValue)
                {
                    query = query.Where(t => t.Category == category.Value);
                }

                var totalCount = await query.CountAsync();

                var pagedTests = await query
                    .OrderByDescending(t => t.CreateAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var items = pagedTests.Select(t => new TestDetailDTO
                {
                    TestID = t.TestID,
                    CreateBy = t.CreateBy,
                    SubjectID = t.SubjectID,
                    CreateAt = t.CreateAt,
                    UpdateAt = (DateTime)t.UpdateAt,
                    Status = t.Status,
                    Category = t.Category,
                    TestType = t.TestType,
                    CreatedByName = t.Account?.FirstName,
                    SubjectName = t.Subject?.SubjectName
                }).ToList();

                return OperationResult<(List<TestDetailDTO>, int)>.Ok((items, totalCount));
            }
            catch (Exception ex)
            {
                return OperationResult<(List<TestDetailDTO>, int)>.Fail($"Error retrieving paginated tests: {ex.Message}");
            }
        }

        private static TestDetailDTO MapToDTO(Test t, string? creatorName = null, string? subjectName = null)
        {
            return new TestDetailDTO
            {
                TestID = t.TestID,
                CreateBy = t.CreateBy,
                SubjectID = t.SubjectID,
                CreateAt = t.CreateAt,
                UpdateAt = (DateTime)t.UpdateAt,
                Status = t.Status,
                Category = t.Category,
                TestType = t.TestType,
                CreatedByName = creatorName ?? t.Account?.FirstName,
                SubjectName = subjectName ?? t.Subject?.SubjectName
            };
        }
    }
}
