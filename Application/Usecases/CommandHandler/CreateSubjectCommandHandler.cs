using Application.IServices;
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
        private readonly ISubjectService _subjectService;

        public CreateSubjectCommandHandler(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public async Task<string> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            var existingSubject = await _subjectService.GetSubjectByIdAsync(request.SubjectID);
            if (existingSubject != null)
            {
                return $"Subject with ID {request.SubjectID} already exists";
            }

            return await _subjectService.CreateSubjectAsync(request);
        }
    }
}