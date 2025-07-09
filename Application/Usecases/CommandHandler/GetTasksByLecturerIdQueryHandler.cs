using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Query;
using Domain.Entities;
using MediatR;

namespace Application.Usecases.QueryHandler
{
    public class GetTasksByLecturerIdQueryHandler : IRequestHandler<GetTasksByLecturerIdQuery, OperationResult<List<WorkTask>>>
    {
        private readonly ITaskService _taskService;

        public GetTasksByLecturerIdQueryHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<OperationResult<List<WorkTask>>> Handle(GetTasksByLecturerIdQuery request, CancellationToken cancellationToken)
        {
            return await _taskService.GetTasksByLecturerIdAsync(request.LecturerId);
        }
    }
}