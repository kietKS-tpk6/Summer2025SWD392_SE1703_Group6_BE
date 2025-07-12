using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandlers
{
    public class CreateStudentMarkFromStudentTestCommandHandler : IRequestHandler<CreateStudentMarkFromStudentTestCommand, OperationResult<string>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public CreateStudentMarkFromStudentTestCommandHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<string>> Handle(CreateStudentMarkFromStudentTestCommand request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.CreateStudentMarkFromStudentTestAsync(request.StudentTestId);
        }
    }
}