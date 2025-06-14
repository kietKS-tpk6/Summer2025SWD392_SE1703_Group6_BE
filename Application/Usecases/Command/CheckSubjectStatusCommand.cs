using MediatR;

namespace Application.Usecases.Command
{
    public class CheckSubjectStatusCommand : IRequest<SubjectStatusCheckResult>
    {
        public string SubjectID { get; set; }
    }

    public class SubjectStatusCheckResult
    {
        public bool CanActivate { get; set; }
        public bool HasSchedule { get; set; }
        public bool HasAssessmentCriteria { get; set; }
        public List<string> MissingFields { get; set; } = new List<string>();
        public string Message { get; set; }
    }
}