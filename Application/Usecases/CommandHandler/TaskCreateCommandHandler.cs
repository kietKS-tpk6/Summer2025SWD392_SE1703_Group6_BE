using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class TaskCreateCommandHandler : IRequestHandler<TaskCreateCommand, OperationResult<string?>>
    {
        private readonly ITaskService _taskService;

        public TaskCreateCommandHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<OperationResult<string?>> Handle(TaskCreateCommand request, CancellationToken cancellationToken)
        {
            return await _taskService.CreateTaskAsync(request);
        }
    }
}