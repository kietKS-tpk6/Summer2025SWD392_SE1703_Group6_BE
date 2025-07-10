using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class StudentMarkDetailDTO
    {
        public AssessmentCategory AssessmentCategory { get; set; }
        public int AttemptNumber { get; set; }
        public List<StudentMarkItem> StudentMarks { get; set; } 

    };
    public class StudentMarkItem
    {
        public string StudentMarkID { get; set; }
        public string StudentName { get; set; }
        public decimal? Mark { get; set; }
        public string? Comment { get; set; }
        public string? StudentTestID { get; set; }
    }
}
