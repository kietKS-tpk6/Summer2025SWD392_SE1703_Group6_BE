using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public QuestionRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Question>> GetQuestionBySectionId(string sectionId)
        {
            return await _dbContext.Question.Where(q => q.TestSectionID == sectionId).ToListAsync();
        }

        public async Task<int> GetTotalQuestionCount()
        {
            return await _dbContext.Question.CountAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Question> questions)
        {
            await _dbContext.Question.AddRangeAsync(questions);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Question question)
        {
            _dbContext.Question.Update(question);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateRangeAsync(IEnumerable<Question> questions)
        {
            _dbContext.Question.UpdateRange(questions);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Question?> GetByIdAsync(string questionId)
        {
            return await _dbContext.Question
                .FirstOrDefaultAsync(q => q.QuestionID == questionId);
        }
    }
}
