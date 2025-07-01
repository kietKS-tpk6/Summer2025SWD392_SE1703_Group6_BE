using Application.Common.Constants;
using Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Application.Usecases.Queries
{
    public class GetStudentMarksByClassAndAssessmentQuery : IRequest<OperationResult<List<StudentMarkDTO>>>
    {
        public string ClassId { get; set; }
        public string AssessmentCriteriaId { get; set; }
    }
}