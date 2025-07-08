using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateTaskStatusCommand : IRequest<OperationResult<string?>>
    {
        public string TaskId { get; set; }
        public string Status { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? Deadline { get; set; }
    }

}