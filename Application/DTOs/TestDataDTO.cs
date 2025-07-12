using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class TestDataDTO
    {
        public string AssessmentID { get; set; }
        public string TestType { get; set; }
        public int TestDurationMinutes { get; set; }
        public bool AllowMultipleAttempts { get; set; }
        public string Category { get; set; }
        public decimal MinPassingScore { get; set; }
    }
}
