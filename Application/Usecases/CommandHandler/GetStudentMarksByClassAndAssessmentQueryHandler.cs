using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Queries;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.QueryHandlers
{
    public class GetStudentMarksByClassAndAssessmentQueryHandler : IRequestHandler<GetStudentMarksByClassAndAssessmentQuery, OperationResult<List<StudentMarkDTO>>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public GetStudentMarksByClassAndAssessmentQueryHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<List<StudentMarkDTO>>> Handle(GetStudentMarksByClassAndAssessmentQuery request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.GetStudentMarksByClassAndAssessmentAsync(request.ClassId, request.AssessmentCriteriaId);
        }
    }
}