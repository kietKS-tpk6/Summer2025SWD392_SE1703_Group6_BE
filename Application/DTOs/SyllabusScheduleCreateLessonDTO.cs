using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SyllabusScheduleCreateLessonDTO
    {
        public string SyllabusScheduleId { get; set; }
        public int Week { get; set; }
        public int DurationMinutes { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
