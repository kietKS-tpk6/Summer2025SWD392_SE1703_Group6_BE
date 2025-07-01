using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class TestByClassDTO
    {
        public string TestEventID { get; set; }
        public string? TestID { get; set; }
        public AssessmentCategory? TestCategory { get; set; }
        public string? TestName { get; set; }
        public string? Description { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public TestType TestType { get; set; }
        public TestEventStatus Status { get; set; }

        public int DurationMinutes { get; set; }
        public int AttemptLimit { get; set; }

        public int TotalSubmittedTests { get; set; }          // tổng số bài nộp
        public int TotalStudentsSubmitted { get; set; }       // số học sinh đã nộp (không tính trùng)
    }

}
