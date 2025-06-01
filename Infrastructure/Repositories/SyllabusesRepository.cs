using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;
using CloudinaryDotNet;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SyllabusesRepository : ISyllabusesRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public SyllabusesRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateSyllabusesAsync(Syllabus syllabus)
        {
            await _dbContext.Syllabus.AddAsync(syllabus);
            await _dbContext.SaveChangesAsync();
            return true;
          }

        public async Task<int> GetNumbeOfSyllabusAsync()
        {

            return await _dbContext.Syllabus.CountAsync();
        }



    }
}
