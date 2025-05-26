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
    public class AccountRepository: IAccountRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public AccountRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AddAsync(Account account)
        {

            _dbContext.Accounts.Add(account);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
