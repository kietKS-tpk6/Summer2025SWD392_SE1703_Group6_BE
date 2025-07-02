using Infrastructure.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Enums;
namespace Infrastructure.Repositories
{
    public class StudentTestRepository : IStudentTestRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public StudentTestRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StudentTest> GetByIdAsync(string studentTestID)
        {
            return await _dbContext.StudentTest
        .Include(st => st.TestEvent) 
        .FirstOrDefaultAsync(x => x.StudentTestID == studentTestID);
        }

        public async Task<bool> ExistsAsync(string studentTestID)
        {
            return await _dbContext.StudentTest.AnyAsync(x => x.StudentTestID == studentTestID);
        }

        public async Task<OperationResult<bool>> UpdateAsync(StudentTest test)
        {
            try
            {
                _dbContext.StudentTest.Update(test);
                await _dbContext.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail("Lỗi cập nhật StudentTest: " + ex.Message);
            }
        }
        public async Task<List<StudentTest>> GetByTestEventIDsAsync(List<string> testEventIDs)
        {
            return await _dbContext.StudentTest
                .Where(st => testEventIDs.Contains(st.TestEventID))
                .ToListAsync();
        }
        public async Task<List<StudentTest>> GetByTestEventIdAsync(string testEventId)
        {
            return await _dbContext.StudentTest
                .Where(st => st.TestEventID == testEventId)
                .ToListAsync();
        }
    }
}
