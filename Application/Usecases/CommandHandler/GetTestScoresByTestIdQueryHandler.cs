using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.QueryHandlers
{
    public class GetTestScoresByTestIdQueryHandler : IRequestHandler<GetTestScoresByTestIdQuery, OperationResult<GetTestScoresDTO>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public GetTestScoresByTestIdQueryHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<GetTestScoresDTO>> Handle(GetTestScoresByTestIdQuery request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.GetTestScoresByTestIdAsync(request.TestId);
        }
    }
}