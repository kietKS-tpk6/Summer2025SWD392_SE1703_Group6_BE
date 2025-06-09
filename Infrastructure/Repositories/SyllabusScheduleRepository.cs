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
    public class SyllabusScheduleRepository : ISyllabusScheduleRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public SyllabusScheduleRepository(HangulLearningSystemDbContext context)
        {
            _dbContext = context;
        }

        public async Task<List<int>> GetWeeksBySubjectIdAsync(string syllabusId)
        {
            var weeks = await _dbContext.SyllabusSchedule
                .Where(s => s.SyllabusID == syllabusId)
                .Select(s => s.Week)
                .ToListAsync();

            return weeks;
        }

        public async Task<List<SyllabusScheduleCreateLessonDTO>> GetPublishedSchedulesBySyllabusIdAsync(string syllabusId)
            {
            var result = await _dbContext.SyllabusSchedule
                .Where(ss => ss.SyllabusID == syllabusId)
                .Select(ss => new SyllabusScheduleCreateLessonDTO
            {
                    SyllabusScheduleId = ss.SyllabusScheduleID,
                    Week = ss.Week,
                    DurationMinutes = ss.DurationMinutes
                })
                .ToListAsync();

            return result;
        }
        public async Task<int> GetNumbeOfSyllabusScheduleAsync()
        {
            return await _dbContext.SyllabusSchedule.CountAsync() + 1;
        }
        public async Task<bool> CreateSyllabusesScheduleAsync(SyllabusSchedule syllabusSchedule)
        {
            try
            {
                _dbContext.SyllabusSchedule.Add(syllabusSchedule);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsMaxSlotInWeek(string syllabusID, int week)
        {
            var count = await _dbContext.SyllabusSchedule
                .Where(s => s.SyllabusID == syllabusID && s.Week == week)
                .CountAsync();

            return count < 8; 
        }
        public async Task<List<SyllabusSchedule>> GetSyllabusSchedulesBySyllabusIdAsync(string syllabusId)
        {
            return await _dbContext.SyllabusSchedule
                .Where(s => s.SyllabusID == syllabusId)
                .ToListAsync();
        }

        public async Task<bool> SlotAllowToTestAsync(string syllabusScheduleID)
        {
            return await _dbContext.SyllabusSchedule
                .AnyAsync(x => x.SyllabusScheduleID == syllabusScheduleID
                            && x.IsActive
                            && x.HasTest == true);
        }


        public async Task<List<(int Week, string TestType)>> GetActiveTestsOrderedByWeekAsync(string syllabusId)
        {
            var tempList = await _dbContext.SyllabusScheduleTests
                .Where(t => t.SyllabusSchedule.SyllabusID == syllabusId && t.SyllabusSchedule.IsActive)
                .OrderBy(t => t.SyllabusSchedule.Week)
                .ThenBy(t => t.SyllabusSchedule.SyllabusScheduleID)
                .Select(t => new
                {
                    Week = t.SyllabusSchedule.Week,
                    TestType = t.TestType.ToString().ToLower()
                })
                .ToListAsync();

            return tempList.Select(t => (t.Week, t.TestType)).ToList();
        }


    }
}
