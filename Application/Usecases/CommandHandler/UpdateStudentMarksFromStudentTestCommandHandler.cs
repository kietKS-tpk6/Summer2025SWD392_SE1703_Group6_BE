using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandlers
{
    public class UpdateStudentMarksFromStudentTestCommandHandler : IRequestHandler<UpdateStudentMarksFromStudentTestCommand, OperationResult<string>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public UpdateStudentMarksFromStudentTestCommandHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<string>> Handle(UpdateStudentMarksFromStudentTestCommand request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.UpdateStudentMarksFromStudentTestAsync(
                request.StudentTestId,
                request.AssessmentCriteriaId,
                request.ClassId);
        }
    }
}