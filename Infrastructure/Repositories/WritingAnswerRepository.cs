using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.IRepositories;

namespace Infrastructure.Repositories
{
    public class WritingAnswerRepository : IWritingAnswerRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public WritingAnswerRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
