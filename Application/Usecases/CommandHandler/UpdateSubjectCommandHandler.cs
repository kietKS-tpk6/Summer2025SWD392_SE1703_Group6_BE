using Infrastructure.IRepositories;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, string>
    {
        private readonly ISubjectRepository _subjectRepository;

        public UpdateSubjectCommandHandler(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<string> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
        {
            var existingSubject = await _subjectRepository.GetSubjectByIdAsync(request.SubjectID);
            if (existingSubject == null)
            {
                return $"Subject with ID {request.SubjectID} not found";
            }

            existingSubject.SubjectName = request.SubjectName;
            existingSubject.Description = request.Description;
            existingSubject.IsActive = request.IsActive;
            existingSubject.MinAverageScoreToPass = request.MinAverageScoreToPass;

            return await _subjectRepository.UpdateSubjectAsync(existingSubject);
        }
    }
}