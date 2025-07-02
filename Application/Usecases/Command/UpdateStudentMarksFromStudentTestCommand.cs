using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Commands
{
    public class UpdateStudentMarksFromStudentTestCommand : IRequest<OperationResult<string>>
    {
        public string StudentTestId { get; set; }
        public string AssessmentCriteriaId { get; set; }
        public string ClassId { get; set; }
    }
}