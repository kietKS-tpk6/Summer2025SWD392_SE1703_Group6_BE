using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Infrastructure.IRepositories;
namespace Infrastructure.Services
{
    public class AccountService: IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<LoginDTO> Login(LoginCommand loginCommand)
        {
            var account = await _accountRepository.LoginAsync(loginCommand);

            if (account == null)
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

            var result = new LoginDTO
            {
                Fullname = $"{account.FirstName} {account.LastName}",
            };

            return result;
        }
    }
}
