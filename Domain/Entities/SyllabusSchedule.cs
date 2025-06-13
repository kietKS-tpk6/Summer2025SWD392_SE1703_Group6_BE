using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("SyllabusSchedules")]
    public class SyllabusSchedule
    {
        [Key]
        [MaxLength(7)]
        public string SyllabusScheduleID { get; set; }

        [MaxLength(6)]
        [ForeignKey("Subject")]
        public string SubjectID { get; set; }

        [MaxLength(255)]
        public string? Content { get; set; }

        public int? Week { get; set; }

        public string? Resources { get; set; }

        [MaxLength(100)]
        public string? LessonTitle { get; set; }

        public int? DurationMinutes { get; set; }

        public bool IsActive { get; set; }
        public bool HasTest { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
