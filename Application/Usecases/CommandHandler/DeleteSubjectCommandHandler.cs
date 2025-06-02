using Infrastructure.IRepositories;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, string>
    {
        private readonly ISubjectRepository _subjectRepository;

        public DeleteSubjectCommandHandler(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<string> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
        {
            var existingSubject = await _subjectRepository.GetSubjectByIdAsync(request.SubjectID);
            if (existingSubject == null)
            {
                return $"Subject with ID {request.SubjectID} not found";
            }

            return await _subjectRepository.DeleteSubjectAsync(request.SubjectID);
        }
    }
}