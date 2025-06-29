using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class UpdateSubjectStatusCommandHandler : IRequestHandler<UpdateSubjectStatusCommand, string>
    {
        private readonly ISubjectService _subjectService;

        public UpdateSubjectStatusCommandHandler(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public async Task<string> Handle(UpdateSubjectStatusCommand request, CancellationToken cancellationToken)
        {
            return await _subjectService.UpdateSubjectStatusAsync(request);
        }
    }
}