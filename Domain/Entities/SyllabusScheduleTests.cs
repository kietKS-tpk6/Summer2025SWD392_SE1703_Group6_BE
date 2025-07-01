using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("SyllabusScheduleTests")]
    public class SyllabusScheduleTest
    {
        [Key]
        [MaxLength(6)] 
        public string ScheduleTestID { get; set; }

        [Required]
        [MaxLength(7)]
        [ForeignKey("SyllabusSchedule")]
        public string SyllabusScheduleID { get; set; }

        [Required]
        [MaxLength(50)] 
        public TestType  TestType { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool AllowMultipleAttempts { get; set; } 

        public int? DurationMinutes { get; set; } 

        [Required]
        [MaxLength(6)] 
        public string AssessmentCriteriaID { get; set; } 

        [Required]
        public virtual SyllabusSchedule SyllabusSchedule { get; set; }

        [ForeignKey("Class")]
        public string ClassID { get; set; }
    }
}
