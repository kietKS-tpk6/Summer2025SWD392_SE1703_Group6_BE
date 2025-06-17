using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Enums;
using MediatR;
namespace Application.Usecases.Command
{
    public class AssessmentCriteriaUpdateCommand : IRequest<OperationResult<AssessmentCriteriaUpdateDto>>
    {
        public string AssessmentCriteriaID { get; set; }
        public double WeightPercent { get; set; }
        public AssessmentCategory Category { get; set; }
        public int RequiredTestCount { get; set; }
        public string? Note { get; set; }
        public decimal MinPassingScore { get; set; }
    }
}
