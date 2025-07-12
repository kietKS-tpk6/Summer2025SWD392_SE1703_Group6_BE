using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Commands
{
    public class CreateStudentMarkFromStudentTestCommand : IRequest<OperationResult<string>>
    {
        public string StudentTestId { get; set; }
    }
}