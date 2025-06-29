using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SyllabusScheduleUpdateItemDTO
    {
        public string SyllabusScheduleID { get; set; }
        public string? Content { get; set; }
        public string? Resources { get; set; }
        public string? LessonTitle { get; set; }
        public int? DurationMinutes { get; set; }
        public bool HasTest { get; set; }
        public SyllabusScheduleTestItemDTO? ItemsAssessmentCriteria { get; set; }
    }
}
