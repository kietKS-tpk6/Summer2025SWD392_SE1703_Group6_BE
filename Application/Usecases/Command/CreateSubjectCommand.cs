using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class CreateSubjectCommand : IRequest<string>
    {
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public double MinAverageScoreToPass { get; set; } = 5.0f;
    }
}


