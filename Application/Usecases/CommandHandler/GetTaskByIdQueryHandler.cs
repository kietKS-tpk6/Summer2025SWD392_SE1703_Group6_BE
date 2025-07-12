using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Query;
using Domain.Entities;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, OperationResult<WorkTask?>>
    {
        private readonly ITaskService _taskService;

        public GetTaskByIdQueryHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<OperationResult<WorkTask?>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            return await _taskService.GetTaskByIdAsync(request.TaskId);
        }
    }
}
