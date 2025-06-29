using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StudentTestResultDTO
    {
        public string StudentTestID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string TestID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? SubmitTime { get; set; }
        public string Status { get; set; }
        public decimal? InitialMark { get; set; } // Điểm ban đầu từ StudentTest
        public decimal? FinalMark { get; set; }   // Điểm chốt từ StudentMarks
        public string? Comment { get; set; }      // Nhận xét từ StudentMarks
        public List<TestSectionWithStudentAnswersDTO> Sections { get; set; }
    }
}
