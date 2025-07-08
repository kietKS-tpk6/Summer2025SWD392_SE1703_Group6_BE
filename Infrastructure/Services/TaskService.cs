using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IScheduleWorkRepository _scheduleWorkRepository;

        public TaskService(ITaskRepository taskRepository, IScheduleWorkRepository scheduleWorkRepository)
        {
            _taskRepository = taskRepository;
            _scheduleWorkRepository = scheduleWorkRepository;
        }

        public async Task<OperationResult<string?>> CreateTaskAsync(TaskCreateCommand command)
        {
            try
            {
                string taskId = await GenerateTaskIdAsync();

                var workTask = new WorkTask
                {
                    TaskID = taskId,
                    Type = command.Type.ToString(),
                    Content = command.Content,
                    DateStart = command.DateStart,
                    Deadline = command.Deadline,
                    Note = command.Note,
                    ResourcesURL = command.ResourcesURL
                };

                var taskResult = await _taskRepository.CreateTaskAsync(workTask);
                if (!taskResult.Success)
                {
                    return OperationResult<string?>.Fail(taskResult.Message);
                }

                string scheduleWorkId = await GenerateScheduleWorkIdAsync();
                var scheduleWork = new ScheduleWork
                {
                    ScheduleWorkID = scheduleWorkId,
                    TaskID = taskId,
                    AccountID = command.LecturerID
                };

                var scheduleResult = await _scheduleWorkRepository.CreateScheduleWorkAsync(scheduleWork);
                if (!scheduleResult.Success)
                {
                    await _taskRepository.DeleteTaskAsync(taskId);
                    return OperationResult<string?>.Fail(scheduleResult.Message);
                }

                return OperationResult<string?>.Ok(taskId, "Tạo task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi tạo task: {ex.Message}");
            }
        }

        public async Task<OperationResult<List<WorkTask>>> GetTasksByLecturerIdAsync(string lecturerId)
        {
            try
            {
                var tasks = await _taskRepository.GetTasksByLecturerIdAsync(lecturerId);
                return OperationResult<List<WorkTask>>.Ok(tasks, "Lấy danh sách task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<List<WorkTask>>.Fail($"Lỗi khi lấy danh sách task: {ex.Message}");
            }
        }

        public async Task<OperationResult<WorkTask?>> GetTaskByIdAsync(string taskId)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(taskId);
                if (task == null)
                {
                    return OperationResult<WorkTask?>.Fail("Không tìm thấy task");
                }
                return OperationResult<WorkTask?>.Ok(task, "Lấy thông tin task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<WorkTask?>.Fail($"Lỗi khi lấy thông tin task: {ex.Message}");
            }
        }

        public async Task<OperationResult<string?>> UpdateTaskStatusAsync(string taskId, string status)
        {
            try
            {
                var result = await _taskRepository.UpdateTaskStatusAsync(taskId, status);
                if (!result.Success)
                {
                    return OperationResult<string?>.Fail(result.Message);
                }
                return OperationResult<string?>.Ok(taskId, "Cập nhật trạng thái task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi cập nhật trạng thái task: {ex.Message}");
            }
        }

        public async Task<OperationResult<List<WorkTask>>> GetAllTasksAsync()
        {
            try
            {
                var tasks = await _taskRepository.GetAllTasksAsync();
                return OperationResult<List<WorkTask>>.Ok(tasks, "Lấy tất cả task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<List<WorkTask>>.Fail($"Lỗi khi lấy tất cả task: {ex.Message}");
            }
        }

        public async Task<OperationResult<string?>> DeleteTaskAsync(string taskId)
        {
            try
            {
                await _scheduleWorkRepository.DeleteByTaskIdAsync(taskId);

                var result = await _taskRepository.DeleteTaskAsync(taskId);
                if (!result.Success)
                {
                    return OperationResult<string?>.Fail(result.Message);
                }
                return OperationResult<string?>.Ok(taskId, "Xóa task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi xóa task: {ex.Message}");
            }
        }

        private async Task<string> GenerateTaskIdAsync()
        {
            var lastTask = await _taskRepository.GetLastTaskAsync();
            if (lastTask == null)
            {
                return "TSK001";
            }

            var lastIdNumber = int.Parse(lastTask.TaskID.Substring(3));
            var newIdNumber = lastIdNumber + 1;
            return $"TSK{newIdNumber:D3}";
        }

        private async Task<string> GenerateScheduleWorkIdAsync()
        {
            var lastScheduleWork = await _scheduleWorkRepository.GetLastScheduleWorkAsync();
            if (lastScheduleWork == null)
            {
                return "SCH001";
            }

            var lastIdNumber = int.Parse(lastScheduleWork.ScheduleWorkID.Substring(3));
            var newIdNumber = lastIdNumber + 1;
            return $"SCH{newIdNumber:D3}";
        }
    }
}