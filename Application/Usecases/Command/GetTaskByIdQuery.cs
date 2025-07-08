using Application.Common.Constants;
using Domain.Entities;
using MediatR;

namespace Application.Usecases.Query
{
    public class GetTaskByIdQuery : IRequest<OperationResult<WorkTask?>>
    {
        public string TaskId { get; set; }
    }
}