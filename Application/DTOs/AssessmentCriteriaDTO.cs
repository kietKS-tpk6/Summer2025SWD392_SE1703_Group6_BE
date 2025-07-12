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
        public int? RequiredTestCount { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }
        public decimal? MinPassingScore { get; set; }
        public AssessmentCategory? Category { get; set; }
    }
}
