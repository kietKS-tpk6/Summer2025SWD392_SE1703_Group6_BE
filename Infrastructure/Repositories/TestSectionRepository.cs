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


    }

}
