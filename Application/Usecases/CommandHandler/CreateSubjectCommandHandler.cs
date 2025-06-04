using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, string>
    {
        private readonly ISubjectService _subjectService;

        public CreateSubjectCommandHandler(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public async Task<string> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            return await _subjectService.CreateSubjectAsync(request);
        }
    }
}