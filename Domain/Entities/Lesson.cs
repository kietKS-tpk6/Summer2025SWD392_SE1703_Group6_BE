using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Lessons")]
    public class Lesson
    {
        [Key]
        [MaxLength(7)]
        public string ClassLessonID { get; set; }

        [ForeignKey("Class")]
        [MaxLength(6)]
        public string ClassID { get; set; }

        [ForeignKey("SyllabusSchedule")]
        [MaxLength(7)]
        public string SyllabusScheduleID { get; set; }

        [ForeignKey("Lecturer")]
        [MaxLength(6)]
        public string LecturerID { get; set; }

        public DateTime StartTime { get; set; }

        public string LinkMeetURL { get; set; }
        public bool IsActive { get; set; }

        public virtual Class Class { get; set; }

        public virtual SyllabusSchedule SyllabusSchedule { get; set; }

        public virtual Account Lecturer { get; set; }
    }
}
