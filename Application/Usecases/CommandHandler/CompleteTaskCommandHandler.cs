using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, OperationResult<string?>>
    {
        private readonly ITaskService _taskService;

        public CompleteTaskCommandHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<OperationResult<string?>> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            return await _taskService.CompleteTaskAsync(request.TaskId, request.LecturerID);
        }
    }
}