using Domain.Enums;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateSubjectCommand : IRequest<string>
    {
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public SubjectStatus Status { get; set; }
        public double MinAverageScoreToPass { get; set; }
    }
}