using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.DTOs
{
    public class SyllabusScheduleDTO
    {
        public string SyllabusScheduleID { get; set; }

        public string Slot { get; set; }
        public string SubjectID { get; set; }

        public string Content { get; set; }

        public int Week { get; set; }

        public string Resources { get; set; }

        public string LessonTitle { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsActive { get; set; }
        public bool HasTest { get; set; }
    }

}
