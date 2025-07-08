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
    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, OperationResult<string?>>
    {
        private readonly ITaskService _taskService;

        public UpdateTaskStatusCommandHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<OperationResult<string?>> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            return await _taskService.UpdateTaskStatusAsync(request.TaskId, request.Status);
        }
    }
}
