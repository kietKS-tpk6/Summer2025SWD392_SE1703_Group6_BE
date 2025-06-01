using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Enums;
namespace Application.Usecases.Command
{
    public class AssessmentCriteriaCreateCommand : IRequest<bool>
    {
        public string SyllabusID { get; set; }
        public float WeightPercent { get; set; }
        public TestCategory Category { get; set; } 
        public int RequiredCount { get; set; }
        public int Duration { get; set; }
        public TestType TestType { get; set; } 
        public string? Note { get; set; }
        public float MinPassingScore { get; set; }
    }
}
