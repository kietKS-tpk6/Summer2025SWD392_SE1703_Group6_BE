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
    [Table("StudentTests")]
    public class StudentTest
    {
        [Key]
        [MaxLength(6)]
        public string StudentTestID { get; set; }

        [MaxLength(6)]
        [ForeignKey("Student")]
        public string StudentID { get; set; }

        [MaxLength(6)]
        [ForeignKey("TestEvent")]
        public string TestEventID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? SubmitTime { get; set; }

        public StudentTestStatus Status { get; set; } 

        [MaxLength(6)]
        [ForeignKey("Lecturer")]  
        public string? GradeBy { get; set; }

        public DateTime? GradeAt { get; set; }

        public int? Score { get; set; }

        public string? Feedback { get; set; }

        // Navigation properties
        public virtual Account Student { get; set; }
        public virtual TestEvent TestEvent { get; set; }
        public virtual Account? Account { get; set; }  
    }
}
