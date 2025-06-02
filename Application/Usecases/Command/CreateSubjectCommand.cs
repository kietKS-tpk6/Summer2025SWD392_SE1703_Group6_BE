using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class CreateSubjectCommand : IRequest<string>
    {
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public double MinAverageScoreToPass { get; set; } = 5.0f;
    }
}


