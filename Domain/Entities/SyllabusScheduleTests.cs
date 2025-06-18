using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("SyllabusScheduleTests")]
    public class SyllabusScheduleTest
    {
        [Key]
        public string ScheduleTestID { get; set; }

        [Required]
        [MaxLength(20)] // nếu đúng là string
        [ForeignKey("SyllabusSchedule")]
        public string SyllabusScheduleID { get; set; }

        [Required]
        public TestType TestType { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public virtual SyllabusSchedule SyllabusSchedule { get; set; }
    }
}
