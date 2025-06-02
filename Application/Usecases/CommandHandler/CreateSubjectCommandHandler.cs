using Infrastructure.IRepositories;
using Application.Usecases.Command;
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, string>
    {
        private readonly ISubjectRepository _subjectRepository;

        public CreateSubjectCommandHandler(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<string> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            // Check if subject already exists
            var existingSubject = await _subjectRepository.GetSubjectByIdAsync(request.SubjectID);
            if (existingSubject != null)
            {
                return $"Subject with ID {request.SubjectID} already exists";
            }

            var subject = new Subject
            {
                SubjectID = request.SubjectID,
                SubjectName = request.SubjectName,
                Description = request.Description,
                IsActive = request.IsActive,
                CreateAt = DateTime.Now,
                MinAverageScoreToPass = request.MinAverageScoreToPass
            };

            return await _subjectRepository.CreateSubjectAsync(subject);
        }
    }
}