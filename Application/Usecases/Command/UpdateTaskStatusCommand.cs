using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateTaskStatusCommand : IRequest<OperationResult<string?>>
    {
        public string TaskId { get; set; }
        public string Status { get; set; }
    }

}