using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Enums;
using MediatR;
namespace Application.Usecases.Command
{
    public class AssessmentCriteriaUpdateCommand : IRequest<OperationResult<bool>>
    {
        public string AssessmentCriteriaID { get; set; }
        public string SubjectID { get; set; }
        public double WeightPercent { get; set; }
        public TestCategory Category { get; set; }
        public int RequiredCount { get; set; }
        public int Duration { get; set; }
        public TestType TestType { get; set; }
        public string? Note { get; set; }
        public double MinPassingScore { get; set; }
    }
}
