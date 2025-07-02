using Application.Common.Constants;
using Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Application.Usecases.Commands
{
    public class BatchUpdateStudentMarksFromStudentTestsCommand : IRequest<OperationResult<BatchUpdateResultDTO>>
    {
        public List<StudentTestUpdateDTO> StudentTests { get; set; }
        public string AssessmentCriteriaId { get; set; }
        public string ClassId { get; set; }
    }
}