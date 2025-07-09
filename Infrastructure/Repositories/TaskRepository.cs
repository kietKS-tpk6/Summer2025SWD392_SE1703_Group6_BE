using Application.Common.Constants;
using Domain.Entities;
using Domain.Enums;
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
                .Include(t => t.ScheduleWorks)
                .Where(t => t.ScheduleWorks.Any(sw => sw.AccountID == lecturerId))
                .OrderByDescending(t => t.DateStart)
                .ToListAsync();
        }

        public async Task<WorkTask?> GetTaskByIdAsync(string taskId)
        {
            return await _dbContext.WorkTasks
                .Include(t => t.ScheduleWorks)
                .FirstOrDefaultAsync(t => t.TaskID == taskId);
        }

        public async Task<OperationResult<string?>> UpdateTaskStatusAsync(string taskId, string status, DateTime? dateStart = null, DateTime? deadline = null)
        {
            try
            {
                var task = await _dbContext.WorkTasks.FirstOrDefaultAsync(t => t.TaskID == taskId);
                if (task == null)
                {
                    return OperationResult<string?>.Fail("Không tìm thấy task");
                }

                if (!Enum.TryParse<Domain.Enums.TaskStatus>(status, true, out var taskStatus))
                {
                    return OperationResult<string?>.Fail("Trạng thái task không hợp lệ");
                }

                task.Status = taskStatus;

                if (dateStart.HasValue)
                {
                    task.DateStart = dateStart.Value;
                }

                if (deadline.HasValue)
                {
                    task.Deadline = deadline.Value;
                }
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
                .Include(t => t.ScheduleWorks)
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

        public async Task<List<WorkTask>> GetTasksToAutoCompleteAsync()
        {
            var autoCompleteTypes = TaskTypeExtensions.GetAutoCompleteTaskTypes()
                .Select(t => t.ToString())
                .ToList();

            return await _dbContext.WorkTasks
                .Where(t => autoCompleteTypes.Contains(t.Type) &&
                           t.Status == Domain.Enums.TaskStatus.InProgress &&
                           t.Deadline <= DateTime.Now)
                .ToListAsync();
        }

        public async Task<OperationResult<int>> BulkUpdateTaskStatusAsync(List<string> taskIds, string status)
        {
            try
            {
                if (!Enum.TryParse<Domain.Enums.TaskStatus>(status, true, out var taskStatus))
                {
                    return OperationResult<int>.Fail("Trạng thái task không hợp lệ");
                }

                var tasks = await _dbContext.WorkTasks
                    .Where(t => taskIds.Contains(t.TaskID))
                    .ToListAsync();

                foreach (var task in tasks)
                {
                    task.Status = taskStatus;
                }

                await _dbContext.SaveChangesAsync();
                return OperationResult<int>.Ok(tasks.Count, $"Cập nhật {tasks.Count} task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<int>.Fail($"Lỗi khi cập nhật hàng loạt task: {ex.Message}");
            }
        }
    }
}