using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Application.IServices;

namespace Application.Usecases.CommandHandler
{
    public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, string>
    {
        private readonly ISubjectService _subjectService;

        public DeleteSubjectCommandHandler(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public async Task<string> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
        {
            return await _subjectService.DeleteSubjectAsync(request.SubjectID);
        }
    }
}