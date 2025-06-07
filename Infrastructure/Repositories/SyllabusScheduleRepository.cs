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
    }
}
