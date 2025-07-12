using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class TestEventWithLessonDTO
    {
        public string TestEventID { get; set; }
        public string? TestID { get; set; }
        public string? Description { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int DurationMinutes { get; set; }
        public TestType  TestType { get; set; }
        public AssessmentCategory? AssessmentCategory { get; set; }
        public TestEventStatus Status { get; set; }
        public string? ScheduleTestID { get; set; }
        public int? AttemptLimit { get; set; }
        public string? Password { get; set; }
        public string ClassLessonID { get; set; }
        public string LessonTitle { get; set; }
        public DateTime LessonStartTime { get; set; }
        public DateTime LessonEndTime { get; set; }
        public int AssessmentIndex { get; set; }
    }

}
