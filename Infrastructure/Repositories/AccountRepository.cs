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
        public async Task<Account?> GetAccountsByEmailAsync(string email)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            if (account  != null) return account;
            return null;
        }
        
            public async Task<Account?> GetAccountsByPhoneAsync(string phone)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if (account != null) return account;
            return null;
        }
        
        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _dbContext.Accounts.ToListAsync();
        }

        public async Task<int> GetNumbeOfAccountsAsync()
        {
            return await _dbContext.Accounts.CountAsync();
        }

       
             public async Task<string?> GetHassPassAccountWithEmailAsync(LoginCommand loginCommand)
        {
            var account = await _dbContext.Accounts
           .FirstOrDefaultAsync(x => x.Email == loginCommand.Email);

            return account.HashPass;
        }
        public async Task<string> RegisterAsync(Account account)
        {
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
            return "Đăng ký thành công.";
        }
    }
}
