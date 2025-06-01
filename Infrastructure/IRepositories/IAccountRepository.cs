using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
namespace Infrastructure.IRepositories
{
    public interface IAccountRepository
    {
        Task<string?> GetHassPassAccountWithEmailAsync(LoginCommand loginCommand);

        Task<Account?> GetAccountsByEmailAsync(string email);

        Task<Account?> GetAccountsByPhoneAsync(string phone);

        Task<string> RegisterAsync(Account account);

        Task<int> GetNumbeOfAccountsAsync();

        Task<List<Account>> GetAllAccountsAsync();
    }
}
