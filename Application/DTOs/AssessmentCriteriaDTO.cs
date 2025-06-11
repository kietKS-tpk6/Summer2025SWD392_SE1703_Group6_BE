using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class AssessmentCriteriaDTO
    {
        public string AssessmentCriteriaID { get; set; }
        public string SubjectID { get; set; }
        public double? WeightPercent { get; set; }
        public int? RequiredCount { get; set; }
        public int? Duration { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }
        public double? MinPassingScore { get; set; }
        public AssessmentCategory? Category { get; set; }
        public TestType? TestType { get; set; }
    }
}
