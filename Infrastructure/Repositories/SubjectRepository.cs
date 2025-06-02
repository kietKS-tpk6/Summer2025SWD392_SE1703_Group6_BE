using Infrastructure.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public SubjectRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateSubjectAsync(Subject subject)
        {
            try
            {
                _dbContext.Subject.Add(subject);
                await _dbContext.SaveChangesAsync();
                return "Subject created successfully";
            }
            catch (Exception ex)
            {
                return $"Error creating subject: {ex.Message}";
            }
        }

        public async Task<Subject?> GetSubjectByIdAsync(string subjectId)
        {
            return await _dbContext.Subject
                .FirstOrDefaultAsync(s => s.SubjectID == subjectId);
        }

        public async Task<List<Subject>> GetAllSubjectsAsync(bool? isActive = null)
        {
            var query = _dbContext.Subject.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            return await query
                .OrderBy(s => s.SubjectID)
                .ToListAsync();
        }

        public async Task<string> UpdateSubjectAsync(Subject subject)
        {
            try
            {
                _dbContext.Subject.Update(subject);
                await _dbContext.SaveChangesAsync();
                return "Subject updated successfully";
            }
            catch (Exception ex)
            {
                return $"Error updating subject: {ex.Message}";
            }
        }

        public async Task<string> DeleteSubjectAsync(string subjectId)
        {
            try
            {
                var subject = await GetSubjectByIdAsync(subjectId);
                if (subject != null)
                {
                    _dbContext.Subject.Remove(subject);
                    await _dbContext.SaveChangesAsync();
                    return "Subject deleted successfully";
                }
                return "Subject not found";
            }
            catch (Exception ex)
            {
                return $"Error deleting subject: {ex.Message}";
            }
        }

        public async Task<bool> SubjectExistsAsync(string subjectId)
        {
            return await _dbContext.Subject
                .AnyAsync(s => s.SubjectID == subjectId);
        }

        public async Task<int> GetTotalSubjectsCountAsync()
        {
            return await _dbContext.Subject.CountAsync();
        }
    }
}