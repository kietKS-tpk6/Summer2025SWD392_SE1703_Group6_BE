using MediatR;

namespace Application.Usecases.Command
{
    public class DeleteSubjectCommand : IRequest<string>
    {
        public string SubjectID { get; set; }
    }
}