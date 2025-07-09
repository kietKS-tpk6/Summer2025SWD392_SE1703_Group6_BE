using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StudentTestResultSimpleDTO
    {
        public string StudentTestID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string TestID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? SubmitTime { get; set; }
        public string Status { get; set; }
        public decimal? OriginalSubmissionScore { get; set; }
        public string? Comment { get; set; }
    }
}
