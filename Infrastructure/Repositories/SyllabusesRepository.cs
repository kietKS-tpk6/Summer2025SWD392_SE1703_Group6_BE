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
        public async Task<bool> ExistsSyllabusAsync(string syllabusId)
        {
            return await _dbContext.Syllabus.AnyAsync(s => s.SyllabusID.Equals(syllabusId));
        }

        public async Task<bool> UpdateSyllabusesAsync(Syllabus syllabus)
        {
            var existingSyllabus = await _dbContext.Syllabus
                .FirstOrDefaultAsync(s => s.SyllabusID == syllabus.SyllabusID);

            if (existingSyllabus == null)
                return false;

            existingSyllabus.SubjectID = syllabus.SubjectID;
            existingSyllabus.UpdateBy = syllabus.UpdateBy;
            existingSyllabus.UpdateAt = syllabus.UpdateAt;
            existingSyllabus.Description = syllabus.Description;
            existingSyllabus.Note = syllabus.Note;
            existingSyllabus.Status = syllabus.Status;

            _dbContext.Syllabus.Update(existingSyllabus);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SyllabusIdExistsAsync(string syllabusId)
        {
            return await _dbContext.Syllabus.AnyAsync(s => s.SyllabusID == syllabusId);
        }
    }
}
