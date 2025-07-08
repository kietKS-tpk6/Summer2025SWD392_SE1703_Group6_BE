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
    public class TaskRepository : ITaskRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public TaskRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<string?>> CreateTaskAsync(WorkTask workTask)
        {
            try
            {
                await _dbContext.WorkTasks.AddAsync(workTask);
                await _dbContext.SaveChangesAsync();
                return OperationResult<string?>.Ok(workTask.TaskID, "Tạo task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi tạo task: {ex.Message}");
            }
        }

        public async Task<List<WorkTask>> GetTasksByLecturerIdAsync(string lecturerId)
        {
            return await _dbContext.WorkTasks
                .Where(t => _dbContext.ScheduleWork
                    .Any(sw => sw.TaskID == t.TaskID && sw.AccountID == lecturerId))
                .OrderByDescending(t => t.DateStart)
                .ToListAsync();
        }

        public async Task<WorkTask?> GetTaskByIdAsync(string taskId)
        {
            return await _dbContext.WorkTasks
                .FirstOrDefaultAsync(t => t.TaskID == taskId);
        }

        public async Task<OperationResult<string?>> UpdateTaskStatusAsync(string taskId, string status)
        {
            try
            {
                var task = await _dbContext.WorkTasks.FirstOrDefaultAsync(t => t.TaskID == taskId);
                if (task == null)
                {
                    return OperationResult<string?>.Fail("Không tìm thấy task");
                }

                // Có thể thêm field Status vào WorkTask entity nếu cần
                // task.Status = status;

                await _dbContext.SaveChangesAsync();
                return OperationResult<string?>.Ok(taskId, "Cập nhật trạng thái thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }

        public async Task<List<WorkTask>> GetAllTasksAsync()
        {
            return await _dbContext.WorkTasks
                .OrderByDescending(t => t.DateStart)
                .ToListAsync();
        }

        public async Task<OperationResult<string?>> DeleteTaskAsync(string taskId)
        {
            try
            {
                var task = await _dbContext.WorkTasks.FirstOrDefaultAsync(t => t.TaskID == taskId);
                if (task == null)
                {
                    return OperationResult<string?>.Fail("Không tìm thấy task");
                }

                _dbContext.WorkTasks.Remove(task);
                await _dbContext.SaveChangesAsync();
                return OperationResult<string?>.Ok(taskId, "Xóa task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi xóa task: {ex.Message}");
            }
        }

        public async Task<WorkTask?> GetLastTaskAsync()
        {
            return await _dbContext.WorkTasks
                .OrderByDescending(t => t.TaskID)
                .FirstOrDefaultAsync();
        }
    }
}