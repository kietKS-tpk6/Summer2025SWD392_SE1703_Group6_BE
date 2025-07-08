using Application.Common.Constants;
using Domain.Entities;
using MediatR;

namespace Application.Usecases.Query
{
    public class GetTasksByLecturerIdQuery : IRequest<OperationResult<List<WorkTask>>>
    {
        public string LecturerId { get; set; }
    }
}
