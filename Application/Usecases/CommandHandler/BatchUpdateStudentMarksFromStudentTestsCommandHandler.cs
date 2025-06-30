using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandlers
{
    public class BatchUpdateStudentMarksFromStudentTestsCommandHandler : IRequestHandler<BatchUpdateStudentMarksFromStudentTestsCommand, OperationResult<BatchUpdateResultDTO>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public BatchUpdateStudentMarksFromStudentTestsCommandHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<BatchUpdateResultDTO>> Handle(BatchUpdateStudentMarksFromStudentTestsCommand request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.BatchUpdateStudentMarksFromStudentTestsAsync(
                request.StudentTests,
                request.AssessmentCriteriaId,
                request.ClassId);
        }
    }
}