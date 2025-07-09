using System.Collections.Immutable;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
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
                    ResourcesURL = command.ResourcesURL,
                    Status = Domain.Enums.TaskStatus.InProgress 
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

                await AutoCompleteExpiredTasksAsync(tasks);

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

                if (task.ShouldAutoCompleteNow)
                {
                    await _taskRepository.UpdateTaskStatusAsync(taskId, Domain.Enums.TaskStatus.Completed.ToString());
                    task.Status = Domain.Enums.TaskStatus.Completed;
                }

                return OperationResult<WorkTask?>.Ok(task, "Lấy thông tin task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<WorkTask?>.Fail($"Lỗi khi lấy thông tin task: {ex.Message}");
            }
        }

        public async Task<OperationResult<string?>> UpdateTaskStatusAsync(string taskId, string status, DateTime? dateStart = null, DateTime? deadline = null)
        {
            try
            {
                if (!Enum.TryParse<Domain.Enums.TaskStatus>(status, true, out var taskStatus))
                {
                    return OperationResult<string?>.Fail("Trạng thái task không hợp lệ");
                }

                var task = await _taskRepository.GetTaskByIdAsync(taskId);
                if (task == null)
                {
                    return OperationResult<string?>.Fail("Không tìm thấy task");
                }

                var result = await _taskRepository.UpdateTaskStatusAsync(taskId, taskStatus.ToString(), dateStart, deadline);
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

        public async Task<OperationResult<string?>> CompleteTaskAsync(string taskId, string lecturerID)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(taskId);
                if (task == null)
                {
                    return OperationResult<string?>.Fail("Không tìm thấy task");
                }

                if (task.Status == Domain.Enums.TaskStatus.Completed)
                {
                    return OperationResult<string?>.Fail("Task đã được hoàn thành");
                }

                if (!Enum.TryParse<Domain.Enums.TaskType>(task.Type, true, out var taskType))
                {
                    return OperationResult<string?>.Fail("Loại task không hợp lệ");
                }

                if (taskType == Domain.Enums.TaskType.Meeting)
                {
                    return OperationResult<string?>.Fail("Task loại Meeting không thể hoàn thành thủ công");
                }

                var result = await _taskRepository.UpdateTaskStatusAsync(taskId, Domain.Enums.TaskStatus.Completed.ToString());
                if (!result.Success)
                {
                    return OperationResult<string?>.Fail(result.Message);
                }

                return OperationResult<string?>.Ok(taskId, "Hoàn thành task thành công");
            }
            catch (Exception ex)
            {
                return OperationResult<string?>.Fail($"Lỗi khi hoàn thành task: {ex.Message}");
            }
        }


        public async Task<OperationResult<List<WorkTask>>> GetAllTasksAsync()
        {
            try
            {
                var tasks = await _taskRepository.GetAllTasksAsync();

                await AutoCompleteExpiredTasksAsync(tasks);

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

        private async Task AutoCompleteExpiredTasksAsync(List<WorkTask> tasks)
        {
            var tasksToComplete = tasks.Where(t => t.ShouldAutoCompleteNow).ToList();

            foreach (var task in tasksToComplete)
            {
                await _taskRepository.UpdateTaskStatusAsync(task.TaskID, Domain.Enums.TaskStatus.Completed.ToString());
                task.Status = Domain.Enums.TaskStatus.Completed;
            }
        }

        public async Task<OperationResult<int>> AutoCompleteExpiredTasksAsync()
        {
            try
            {
                var allTasks = await _taskRepository.GetAllTasksAsync();
                var tasksToComplete = allTasks.Where(t => t.ShouldAutoCompleteNow).ToList();

                int completedCount = 0;
                foreach (var task in tasksToComplete)
                {
                    var result = await _taskRepository.UpdateTaskStatusAsync(task.TaskID, Domain.Enums.TaskStatus.Completed.ToString());
                    if (result.Success)
                    {
                        completedCount++;
                    }
                }

                return OperationResult<int>.Ok(completedCount, $"Đã tự động hoàn thành {completedCount} task");
            }
            catch (Exception ex)
            {
                return OperationResult<int>.Fail($"Lỗi khi tự động hoàn thành task: {ex.Message}");
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