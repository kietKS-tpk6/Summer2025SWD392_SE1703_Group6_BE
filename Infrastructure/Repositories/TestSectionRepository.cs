using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace Infrastructure.Repositories
{
    public class TestSectionRepository : ITestSectionRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public TestSectionRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(string testSectionId)
        {
            return await _dbContext.TestSection.AnyAsync(t => t.TestSectionID == testSectionId);
        }

        public async Task<TestFormatType?> GetTestSectionTypeAsync(string testSectionId)
        {
            var sectionTypeStr = await _dbContext.TestSection
                .Where(x => x.TestSectionID == testSectionId)
                .Select(x => x.TestSectionType.ToString())
                .FirstOrDefaultAsync();

            if (Enum.TryParse<TestFormatType>(sectionTypeStr, out var result))
                return result;

            return null; 
        }

        public async Task<decimal?> GetScoreByTestSectionIdAsync(string testSectionId)
        {
            return await _dbContext.TestSection
                .Where(s => s.TestSectionID == testSectionId)
                .Select(s => (decimal?)s.Score)
                .FirstOrDefaultAsync();
        }
        public async Task<List<TestSection>> GetByTestIdAsync(string testId)
        {
            return await _dbContext.TestSection
                .Where(ts => ts.TestID == testId && ts.IsActive)
                .ToListAsync();
        }

        public async Task<OperationResult<string>> CreateTestSectionAsync(TestSection testSection)
        {
            try
            {
                _dbContext.TestSection.Add(testSection);
                await _dbContext.SaveChangesAsync();
                return OperationResult<string>.Ok(testSection.TestSectionID, OperationMessages.CreateSuccess("Test Section"));
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error creating test section: {ex.Message}");
            }
        }

        public async Task<OperationResult<TestSection>> GetTestSectionByIdAsync(string testSectionId)
        {
            try
            {
                var testSection = await _dbContext.TestSection
                    .Include(ts => ts.Test)
                    .FirstOrDefaultAsync(ts => ts.TestSectionID == testSectionId && ts.IsActive);

                if (testSection == null)
                    return OperationResult<TestSection>.Fail(OperationMessages.NotFound("Test Section"));

                return OperationResult<TestSection>.Ok(testSection);
            }
            catch (Exception ex)
            {
                return OperationResult<TestSection>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<List<TestSection>>> GetTestSectionsByTestIdAsync(string testId)
        {
            try
            {
                var testSections = await _dbContext.TestSection
                    .Where(ts => ts.TestID == testId && ts.IsActive)
                    .ToListAsync();

                return OperationResult<List<TestSection>>.Ok(testSections);
            }
            catch (Exception ex)
            {
                return OperationResult<List<TestSection>>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<string>> UpdateTestSectionAsync(TestSection testSection)
        {
            try
            {
                _dbContext.TestSection.Update(testSection);
                await _dbContext.SaveChangesAsync();
                return OperationResult<string>.Ok("", OperationMessages.UpdateSuccess("Test Section"));
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error updating test section: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> DeleteTestSectionAsync(string testSectionId)
        {
            try
            {
                var testSection = await _dbContext.TestSection.FindAsync(testSectionId);
                if (testSection == null)
                    return OperationResult<string>.Fail(OperationMessages.NotFound("Test Section"));

                testSection.IsActive = false;
                await _dbContext.SaveChangesAsync();

                return OperationResult<string>.Ok("", OperationMessages.DeleteSuccess("Test Section"));
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error deleting test section: {ex.Message}");
            }
        }

        public async Task<string> GenerateNextTestSectionIdAsync()
        {
            var count = await _dbContext.TestSection.CountAsync();
            return $"TS{(count + 1):D4}";
        }
        public async Task<List<TestSection>> GetByTestIDAndTypeAsync(string testID, TestFormatType type)
        {
            return await _dbContext.TestSection
                .Where(s => s.TestID == testID && s.TestSectionType == type && s.IsActive)
                .ToListAsync();
        }
        public async Task<decimal> GetTotalScoreBySectionID(string testSectionID)
        {
            return await _dbContext.TestSection
                .Where(q => q.TestSectionID == testSectionID && q.IsActive)
                .SumAsync(q => (decimal?)q.Score) ?? 0;
        }
    }

}
