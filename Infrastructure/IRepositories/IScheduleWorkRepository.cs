using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IScheduleWorkRepository
    {
        Task<OperationResult<string?>> CreateScheduleWorkAsync(ScheduleWork scheduleWork);
        Task<List<ScheduleWork>> GetScheduleWorksByAccountIdAsync(string accountId);
        Task<ScheduleWork?> GetScheduleWorkByIdAsync(string scheduleWorkId);
        Task<OperationResult<string?>> DeleteScheduleWorkAsync(string scheduleWorkId);
        Task<OperationResult<string?>> DeleteByTaskIdAsync(string taskId);
        Task<ScheduleWork?> GetLastScheduleWorkAsync();
    }
}