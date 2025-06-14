using Domain.Enums;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateSubjectStatusCommand : IRequest<string>
    {
        public string SubjectID { get; set; }
        public SubjectStatus Status { get; set; }
    }
}