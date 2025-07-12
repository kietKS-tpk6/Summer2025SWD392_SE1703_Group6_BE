using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class StudentMarkForStudentDTO
    {
      public decimal?  GPA { get; set; }
      public List<MarkComponentDTO> StudentMarkDetails { get; set; }
    }
    public class MarkComponentDTO
    {
        public string StudentMarkID { get; set; }
        public AssessmentCategory? AssessmentCategory { get; set; }
        public double? WeightPercent { get; set; }
        public int AssessmentIndex { get; set; }
        public decimal? Mark { get; set; }
        public string? Comment { get; set; }
        public string? StudentTestID { get; set; }
    }
}
