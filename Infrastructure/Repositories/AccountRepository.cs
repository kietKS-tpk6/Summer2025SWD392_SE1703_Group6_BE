using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public AccountRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //public async Task<bool> AddAsync(Account account)
        //{

        //    _dbContext.Accounts.Add(account);
        //    return await _dbContext.SaveChangesAsync() > 0;
        //}
        public async Task<Account?> LoginAsync (LoginCommand loginCommand)
        {
            var account = await _dbContext.Accounts
           .FirstOrDefaultAsync(x => x.Email == loginCommand.Email);

            if (account == null)
                return null;

            //var passwordHasher = new PasswordHasher<Account>();
            //var result = passwordHasher.VerifyHashedPassword(account, account.PasswordHash, loginCommand.Password);
            //if (result == PasswordVerificationResult.Failed)
            //    return null;

            return account;
        }
    }
}
