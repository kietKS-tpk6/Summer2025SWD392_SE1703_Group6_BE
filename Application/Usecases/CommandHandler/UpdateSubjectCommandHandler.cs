using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Application.IServices;

namespace Application.Usecases.CommandHandler
{
    public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, string>
    {
        private readonly ISubjectService _subjectService;

        public UpdateSubjectCommandHandler(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public async Task<string> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
        {
            return await _subjectService.UpdateSubjectAsync(request);
        }
    }
}