using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface ITaskRepository
    {
        Task<OperationResult<string?>> CreateTaskAsync(WorkTask workTask);
        Task<List<WorkTask>> GetTasksByLecturerIdAsync(string lecturerId);
        Task<WorkTask?> GetTaskByIdAsync(string taskId);
        Task<OperationResult<string?>> UpdateTaskStatusAsync(string taskId, string status, DateTime? dateStart = null, DateTime? deadline = null);
        Task<List<WorkTask>> GetAllTasksAsync();
        Task<OperationResult<string?>> DeleteTaskAsync(string taskId);
        Task<WorkTask?> GetLastTaskAsync();
        Task<List<WorkTask>> GetTasksToAutoCompleteAsync();
        Task<OperationResult<int>> BulkUpdateTaskStatusAsync(List<string> taskIds, string status);
    }
}