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
    public class MCQAnswerDetailRepository : IMCQAnswerDetailRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public MCQAnswerDetailRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
      

        public async Task<List<MCQAnswerDetail>> GetByMCQAnswerIdAsync(string mcqAnswerId)
        {
            return await _dbContext.MCQAnswerDetails
                .Where(mad => mad.MCQAnswerID == mcqAnswerId)
                .ToListAsync();
        }
    }
}
