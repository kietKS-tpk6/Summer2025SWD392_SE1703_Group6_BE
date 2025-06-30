using Application.Common.Constants;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Queries
{
    public class GetTestScoresByTestIdQuery : IRequest<OperationResult<GetTestScoresDTO>>
    {
        public string TestId { get; set; }
    }
}