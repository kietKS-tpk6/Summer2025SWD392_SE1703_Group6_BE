using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;
using CloudinaryDotNet;
using Domain.Entities;
using Domain.Enums;
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

            existingSyllabus.UpdateBy = syllabus.UpdateBy;
            existingSyllabus.UpdateAt = syllabus.UpdateAt;
            existingSyllabus.Description = syllabus.Description;
            existingSyllabus.Note = syllabus.Note;
            existingSyllabus.Status = syllabus.Status;

            _dbContext.Syllabus.Update(existingSyllabus);
            await _dbContext.SaveChangesAsync();

            return true;
        }
      

        public async Task<bool> IsValidSyllabusStatusForSubjectAsync(string subjectId)
        {
            return await _dbContext.Syllabus
                .AnyAsync(s => s.SubjectID == subjectId &&
                               (s.Status.Equals( "Drafted") || s.Status.Equals("Published")));
        }

        public async Task<SyllabusDTO> getSyllabusBySubjectID(string subjectId)
        {
            return await _dbContext.Syllabus
                .Where(s => s.SubjectID == subjectId)
                .Select(s => new SyllabusDTO
                {
                    SyllabusID = s.SyllabusID,
                    SubjectID = s.SubjectID,
                    CreateBy = s.CreateBy,
                    CreateAt = s.CreateAt,
                    UpdateBy = s.UpdateBy,
                    UpdateAt = s.UpdateAt,
                    Description = s.Description,
                    Note = s.Note,
                    Status = s.Status.ToString() // enum thành string
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> deleteSyllabusById(string syllabusID)
        {
            var syllabus = await _dbContext.Syllabus
                .FirstOrDefaultAsync(s => s.SyllabusID == syllabusID);

            if (syllabus == null)
                return false;

            syllabus.Status = SyllabusStatus.Deleted;
            syllabus.UpdateAt = DateTime.UtcNow; 

            _dbContext.Syllabus.Update(syllabus);
            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}
