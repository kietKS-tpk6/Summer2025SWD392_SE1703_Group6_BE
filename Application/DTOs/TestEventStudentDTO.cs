using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class TestEventStudentDTO
    {
      public string ClassName { get; set; }
      public string SubjectName { get; set; }
      public string ClassID { get; set; }
      public List<TestEventInClassDTO> TestEvents { get; set; } 
    }
    public class TestEventInClassDTO
    {
        public string TestEventID { get; set; }
        public string? TestID { get; set; }
        public string? Description { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int DurationMinutes { get; set; }
        public string TestType { get; set; }
        public TestEventStatus Status { get; set; }
        public int? AttemptLimit { get; set; }
        public string? Password { get; set; }
        public string LessonTitle { get; set; }
        public int AssessmentIndex { get; set; }
    }

}
