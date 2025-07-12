using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Query;
using Domain.Entities;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, OperationResult<List<WorkTask>>>
    {
        private readonly ITaskService _taskService;

        public GetAllTasksQueryHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<OperationResult<List<WorkTask>>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            return await _taskService.GetAllTasksAsync();
        }
    }
}
