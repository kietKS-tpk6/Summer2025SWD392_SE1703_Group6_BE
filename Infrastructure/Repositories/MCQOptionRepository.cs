using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using Application.DTOs;
using Application.Common.Constants;
namespace Infrastructure.Repositories
{
    public class MCQOptionRepository : IMCQOptionRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public MCQOptionRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MCQOption>> GetByQuestionIdAsync(string questionId)
        {
            return await _dbContext.MCQOption
                .Where(o => o.QuestionID == questionId)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<MCQOption> options)
        {
            await _dbContext.MCQOption.AddRangeAsync(options);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteByQuestionIdAsync(string questionId)
        {
            var existingOptions = await _dbContext.MCQOption
                .Where(o => o.QuestionID == questionId)
                .ToListAsync();

            _dbContext.MCQOption.RemoveRange(existingOptions);
            await _dbContext.SaveChangesAsync();
        }
    }

}
