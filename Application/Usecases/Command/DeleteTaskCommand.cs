using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class DeleteTaskCommand : IRequest<OperationResult<string?>>
    {
        public string TaskId { get; set; }
    }
}