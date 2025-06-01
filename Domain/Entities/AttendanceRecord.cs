using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
namespace Domain.Entities
{
    [Table("AttendanceRecords")]
    public class AttendanceRecord
    {
        [Key]
        [MaxLength(8)]
        public string AttendaceID { get; set; }

        [ForeignKey("Student")]
        [MaxLength(6)]
        public string StudentID { get; set; }

        [ForeignKey("Lesson")]
        [MaxLength(7)]
        public string ClassLessonID { get; set; }

        [MaxLength(255)]
        public string Note { get; set; }

        public AttendanceStatus Status { get; set; }

        public virtual Account Student { get; set; }

        public virtual Lesson Lesson { get; set; }
    }
}
