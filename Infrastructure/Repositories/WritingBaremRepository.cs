using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;

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
    }

}
