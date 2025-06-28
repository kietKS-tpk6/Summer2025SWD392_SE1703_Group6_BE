using MediatR;
using Domain.Enums;

namespace Application.Usecases.Command
{
    public class CreateTestCommand : IRequest<string>
    {
        public string AccountID { get; set; }
        public string SubjectID { get; set; }
        public TestType TestType { get; set; }
        public AssessmentCategory Category { get; set; }
    }
}