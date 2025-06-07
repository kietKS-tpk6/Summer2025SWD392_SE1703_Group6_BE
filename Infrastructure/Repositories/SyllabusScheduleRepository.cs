using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SyllabusScheduleRepository : ISyllabusScheduleRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public SyllabusScheduleRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public async Task<int> GetNumbeOfSyllabusScheduleAsync()
        {
            return await _dbContext.SyllabusSchedule.CountAsync()+1;
        }
    }
}
