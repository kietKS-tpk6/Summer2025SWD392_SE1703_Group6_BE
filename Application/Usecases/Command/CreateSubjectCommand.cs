using Application.Common.Constants;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class CreateSubjectCommand : IRequest<OperationResult<string>>
    {
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public double MinAverageScoreToPass { get; set; }
    }
}


