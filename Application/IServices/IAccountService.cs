using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Enums;

namespace Application.IServices
{
    public interface IAccountService
    {
        public Task<LoginDTO> Login(LoginCommand loginCommand);
        Task<OperationResult<AccountDTO>> GetAccountByIdAsync(string accountId);

        public Task<bool> Register(RegisterCommand registerCommand);

        public Task<bool> VerifyOTPByEmail(VerifyOTPCommand verifyOTPCommand);

        public Task<bool> CreateAccountByManager(CreateAccountCommand createAccountCommand);

        public Task<PagedResult<AccountForManageDTO>> GetPaginatedAccountListAsync(int page, int pageSize, string? role, string? gender, string? status);
        public Task<bool> CheckEmailExistAsync(string email);

        Task<OperationResult<bool>> IsLectureFreeAsync(string lecturerId, string subjectId, TimeOnly time, List<DayOfWeek> days);

        public Task<bool> CheckPhoneExistAsync(string phone);

        public Task<string> GetAccountNameByIDAsync(string accountID);
        //Hàm của Kho
        Task<OperationResult<List<TeachingScheduleDTO>>> GetTeachingSchedule();
        //Hàm của Kho
        Task<OperationResult<List<TeachingScheduleDetailDTO>>> GetTeachingScheduleDetailByID(string accountID);
        Task<OperationResult<List<AccountDTO>>> GetListAccountByRoleAsync(AccountRole accountRole);
        Task<OperationResult<AccountDTO>> UpdateAccountAsync(UpdateAccountCommand command);
        Task<OperationResult<List<AccountDTO>>> SearchAccountsAsync(SearchAccountsQueryCommand command);

        Task<OperationResult<List<AccountDTO>>> GetFreeLecturersAsync(CheckLecturerFreeCommand request);
    }
}
