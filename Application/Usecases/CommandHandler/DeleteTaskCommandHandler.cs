using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, OperationResult<string?>>
    {
        private readonly ITaskService _taskService;

        public DeleteTaskCommandHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<OperationResult<string?>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            return await _taskService.DeleteTaskAsync(request.TaskId);
        }
    }
}
