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

        public async Task<bool> AddAsync(SyllabusScheduleTest entity)
        {
            _dbContext.SyllabusScheduleTests.Add(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<SyllabusScheduleTestDTO>> GetTestsBySyllabusIdAsync(string syllabusId)
        {
            var result = await _dbContext.SyllabusScheduleTests
                .Where(t => t.SyllabusSchedule.SyllabusID == syllabusId && t.IsActive == true)
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

       
    }
}
