using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AssessmentCriteriaUpdateDTO
    {
        public int Order { get; set; }
        public string AssessmentCriteriaID { get; set; }
        public string? Category { get; set; }
        public int? RequireCount { get; set; }
    }
}
