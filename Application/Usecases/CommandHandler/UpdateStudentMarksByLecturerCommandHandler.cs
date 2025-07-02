using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandlers
{
    public class UpdateStudentMarksByLecturerCommandHandler : IRequestHandler<UpdateStudentMarksByLecturerCommand, OperationResult<bool>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public UpdateStudentMarksByLecturerCommandHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<bool>> Handle(UpdateStudentMarksByLecturerCommand request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.UpdateStudentMarksByLecturerAsync(
                request.StudentMarkId,
                request.Mark,
                request.Comment,
                request.LecturerId);
        }
    }
}