using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ScheduleWorkRepository : IScheduleWorkRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public ScheduleWorkRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<string?>> CreateScheduleWorkAsync(ScheduleWork scheduleWork)
        {
            try
            {
                await _dbContext.ScheduleWork.AddAsync(scheduleWork);
                await _dbContext.SaveChangesAsync();
                return OperationResult<string?>.Ok(scheduleWork.ScheduleWorkID, "Tạo schedule work thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi tạo schedule work: {ex.Message}");
            }
        }

        public async Task<List<ScheduleWork>> GetScheduleWorksByAccountIdAsync(string accountId)
        {
            return await _dbContext.ScheduleWork
                .Include(sw => sw.WorkTask)
                .Include(sw => sw.Account)
                .Where(sw => sw.AccountID == accountId)
                .ToListAsync();
        }

        public async Task<ScheduleWork?> GetScheduleWorkByIdAsync(string scheduleWorkId)
        {
            return await _dbContext.ScheduleWork
                .Include(sw => sw.WorkTask)
                .Include(sw => sw.Account)
                .FirstOrDefaultAsync(sw => sw.ScheduleWorkID == scheduleWorkId);
        }

        public async Task<OperationResult<string?>> DeleteScheduleWorkAsync(string scheduleWorkId)
        {
            try
            {
                var scheduleWork = await _dbContext.ScheduleWork.FirstOrDefaultAsync(sw => sw.ScheduleWorkID == scheduleWorkId);
                if (scheduleWork == null)
                {
                    return OperationResult<string?>.Fail("Không tìm thấy schedule work");
                }

                _dbContext.ScheduleWork.Remove(scheduleWork);
                await _dbContext.SaveChangesAsync();
                return OperationResult<string?>.Ok(scheduleWorkId, "Xóa schedule work thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi xóa schedule work: {ex.Message}");
            }
        }

        public async Task<OperationResult<string?>> DeleteByTaskIdAsync(string taskId)
        {
            try
            {
                var scheduleWorks = await _dbContext.ScheduleWork.Where(sw => sw.TaskID == taskId).ToListAsync();
                if (scheduleWorks.Any())
                {
                    _dbContext.ScheduleWork.RemoveRange(scheduleWorks);
                    await _dbContext.SaveChangesAsync();
                }
                return OperationResult<string?>.Ok(taskId, "Xóa schedule works thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi xóa schedule works: {ex.Message}");
            }
        }

        public async Task<ScheduleWork?> GetLastScheduleWorkAsync()
        {
            return await _dbContext.ScheduleWork
                .OrderByDescending(sw => sw.ScheduleWorkID)
                .FirstOrDefaultAsync();
        }
    }
}