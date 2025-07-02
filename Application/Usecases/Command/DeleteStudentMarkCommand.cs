using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Commands
{
    public class DeleteStudentMarkCommand : IRequest<OperationResult<bool>>
    {
        public string StudentMarkId { get; set; }
    }
}