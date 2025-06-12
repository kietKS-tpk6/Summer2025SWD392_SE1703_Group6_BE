using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
namespace Infrastructure.IRepositories
{
    public interface IAccountRepository
    {
        Task<string?> GetHassPassAccountWithEmailAsync(LoginCommand loginCommand);

        Task<Account?> GetAccountsByEmailAsync(string email);

        Task<Account?> GetAccountsByPhoneAsync(string phone);

        Task<bool> CreateAccountAsync(Account account);

        Task<int> GetNumbeOfAccountsAsync();
        Task<(List<AccountForManageDTO> Items, int TotalCount)> GetPaginatedAccountListAsync(int page, int pageSize, AccountRole? role = null, Gender? gender = null, AccountStatus? status = null);
        Task<List<Account>> GetAllAccountsAsync();
        Task<string> GetAccountNameByIDAsync(string accountID);


        Task<OperationResult<bool>> IsLectureFreeAsync(string lecturerId, string subjectId, TimeOnly time, List<DayOfWeek> days);
        Task<OperationResult<List<TeachingScheduleDTO>>> GetTeachingSchedule();
    }
}
