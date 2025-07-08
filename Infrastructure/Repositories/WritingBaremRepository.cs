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
    public class WritingBaremRepository : IWritingBaremRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public WritingBaremRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRangeAsync(List<WritingBarem> barems)
        {
            await _dbContext.WritingBarem.AddRangeAsync(barems);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<WritingBarem>> GetByQuestionIDAsync(string questionID)
        {
            return await _dbContext.WritingBarem
                .Where(b => b.QuestionID == questionID && b.IsActive)
                .ToListAsync();
        }

        public async Task<WritingBarem?> GetByIDAsync(string writingBaremID)
        {
            return await _dbContext.WritingBarem
                .FirstOrDefaultAsync(x => x.WritingBaremID == writingBaremID && x.IsActive);
        }

        public async Task UpdateAsync(WritingBarem barem)
        {
            _dbContext.WritingBarem.Update(barem);
            await _dbContext.SaveChangesAsync();
        }
    }
}
