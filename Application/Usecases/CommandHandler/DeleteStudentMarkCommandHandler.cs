using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandlers
{
    public class DeleteStudentMarkCommandHandler : IRequestHandler<DeleteStudentMarkCommand, OperationResult<bool>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public DeleteStudentMarkCommandHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<bool>> Handle(DeleteStudentMarkCommand request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.DeleteStudentMarkAsync(request.StudentMarkId);
        }
    }
}