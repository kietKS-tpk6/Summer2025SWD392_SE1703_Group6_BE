using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.IRepositories;

namespace Infrastructure.Repositories
{
    public class QuestionsRepository : IQuestionsRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public QuestionsRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }


    }
}
