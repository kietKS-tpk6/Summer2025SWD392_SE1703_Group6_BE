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

        public async Task<List<SyllabusScheduleTestDTO>> GetTestsBySyllabusIdAsync(string syllabusId)
        {
            var result = await _dbContext.SyllabusScheduleTests
                .Where(t => t.SyllabusSchedule.SyllabusID == syllabusId)
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

      


    }
}
