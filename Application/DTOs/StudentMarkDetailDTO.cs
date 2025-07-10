using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StudentMarkDetailDTO
    {
        public string StudentMarkID { get; set; }
        public string AssessmentCriteriaID { get; set; }
        public decimal? Mark { get; set; }
        public string Comment { get; set; }
        public string GradedBy { get; set; }
        public DateTime? GradedAt { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
