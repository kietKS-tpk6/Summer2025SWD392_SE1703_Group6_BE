using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Entities;

namespace Application.IServices
{
    public interface ITaskService
    {
        Task<OperationResult<string?>> CreateTaskAsync(TaskCreateCommand command);
        Task<OperationResult<List<WorkTask>>> GetTasksByLecturerIdAsync(string lecturerId);
        Task<OperationResult<WorkTask?>> GetTaskByIdAsync(string taskId);
        Task<OperationResult<string?>> UpdateTaskStatusAsync(string taskId, string status);
        Task<OperationResult<string?>> CompleteTaskAsync(string taskId, string lecturerID);
        Task<OperationResult<List<WorkTask>>> GetAllTasksAsync();
        Task<OperationResult<string?>> DeleteTaskAsync(string taskId);
        Task<OperationResult<int>> AutoCompleteExpiredTasksAsync();

    }
}