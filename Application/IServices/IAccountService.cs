using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IAccountService
    {
        public Task<LoginDTO> Login(LoginCommand loginCommand);

        public Task<bool> Register(RegisterCommand registerCommand);

        public Task<bool> VerifyOTPByEmail(VerifyOTPCommand verifyOTPCommand);

        public Task<bool> CreateAccountByManager(CreateAccountCommand createAccountCommand);

        public Task<PagedResult<AccountForManageDTO>> GetPaginatedAccountListAsync(int page, int pageSize, string? role, string? gender, string? status);



    }
}
