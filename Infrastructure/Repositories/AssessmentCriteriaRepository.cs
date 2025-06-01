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
    public class AssessmentCriteriaRepository : IAssessmentCriteriaRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public AssessmentCriteriaRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AssessmentCriteria>> GetAllAsync()
        {
            return await _dbContext.AssessmentCriteria.ToListAsync();
        }

        public async Task<AssessmentCriteria?> GetAsync(string id)
        {
            return await _dbContext.AssessmentCriteria.FindAsync(id);
        }

        public async Task<bool> CreateAsync(AssessmentCriteria assessmentCriteria)
        {
            _dbContext.AssessmentCriteria.Add(assessmentCriteria);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(AssessmentCriteria assessmentCriteria)
        {
            _dbContext.AssessmentCriteria.Update(assessmentCriteria);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0; 
        }

        public async Task<bool> DeleteAsync(AssessmentCriteria assessmentCriteria)
        {
            _dbContext.AssessmentCriteria.Remove(assessmentCriteria);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<int> CountAsync()
        {
            return await _dbContext.AssessmentCriteria.CountAsync();
        }
    }
}
