using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using Application.DTOs;
namespace Infrastructure.Repositories
{
    public class SyllabusScheduleTestRepository : ISyllabusScheduleTestRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public SyllabusScheduleTestRepository(HangulLearningSystemDbContext context)
        {
            _dbContext = context;
        }

        public SyllabusScheduleTestRepository()
        {
        }

        public async Task<bool> AddAsync(SyllabusScheduleTest entity)
        {
            _dbContext.SyllabusScheduleTests.Add(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(SyllabusScheduleTest entity)
        {
            _dbContext.SyllabusScheduleTests.Update(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<SyllabusScheduleTestDTO>> GetTestsBySyllabusIdAsync(string subjectID)
        {
            var result = await _dbContext.SyllabusScheduleTests
                .Where(t => t.SyllabusSchedule.SubjectID == subjectID && t.IsActive == true)
                .Select(t => new SyllabusScheduleTestDTO
                {
                    ID = t.ID,
                    SyllabusSchedulesID = t.SyllabusSchedulesID,
                    TestCategory = t.TestCategory.ToString(),
                    TestType = t.TestType.ToString()
                })
                .ToListAsync();

            return result;
        }
        public async Task<bool> HasTestAsync(string syllabusScheduleId)
        {
            return await _dbContext.SyllabusScheduleTests
                .AnyAsync(t => t.SyllabusSchedulesID == syllabusScheduleId && t.IsActive);
        }

        public async Task<bool> RemoveTestFromSlotAsyncs(string syllabusScheduleId)
        {
            var syllabusSchedule = syllabusScheduleId;
            var testsToUpdate = await _dbContext.SyllabusScheduleTests
                .Where(t => t.SyllabusSchedulesID == syllabusScheduleId && t.IsActive)
                .ToListAsync();

            if (!testsToUpdate.Any())
                return false;

            foreach (var test in testsToUpdate)
            {
                test.IsActive = false;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

      
        public async Task<SyllabusScheduleTest> GetByIdAsync(int SyllabusScheduleTestID)
        {
            return await _dbContext.SyllabusScheduleTests
                            .FirstOrDefaultAsync(x => x.ID == SyllabusScheduleTestID);
        }

        public async Task<List<SyllabusScheduleTestDTO>> GetExamAddedToSyllabusAsync(List<string> slotAllowToTest)
        {
            var query = _dbContext.SyllabusScheduleTests
                .Where(x => x.IsActive == true);

            if (slotAllowToTest != null && slotAllowToTest.Count > 0)
            {
                // Lọc các SyllabusScheduleTests có SyllabusSchedulesID nằm trong danh sách slotAllowToTest
                query = query.Where(x => slotAllowToTest.Contains(x.SyllabusSchedulesID));
            }

            var result = await query
                 .Select(x => new SyllabusScheduleTestDTO
                 {
                     ID = x.ID,
                     SyllabusSchedulesID = x.SyllabusSchedulesID,
                     TestCategory = x.TestCategory.ToString(),  // Lấy trực tiếp string
                     TestType = x.TestType.ToString(),          // Lấy trực tiếp string  
                     IsActive = x.IsActive
                 })
                 .ToListAsync();

            return result;
        }
    }
}
