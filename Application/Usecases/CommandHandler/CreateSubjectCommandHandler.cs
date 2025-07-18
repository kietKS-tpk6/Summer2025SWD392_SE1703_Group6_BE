using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, OperationResult<string>>
    {
        private readonly ISubjectService _subjectService;

        public CreateSubjectCommandHandler(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public async Task<OperationResult<string>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            //if (await _subjectService.DescriptionExistsAsync(request.Description))
            //{
            //    return OperationResult<string>.Fail(OperationMessages.AlreadyExists("Mô tả môn học"));
            //}
            if (await _subjectService.SubjectNameExistsAsync(request.SubjectName))
            {
                return OperationResult<string>.Fail(OperationMessages.AlreadyExists("Tên môn học"));
            }

            return await _subjectService.CreateSubjectAsync(new Subject
            {
                SubjectName = request.SubjectName,
                Description = request.Description,
                CreateAt = DateTime.Now,
                MinAverageScoreToPass = request.MinAverageScoreToPass
            });
        }

    }
}