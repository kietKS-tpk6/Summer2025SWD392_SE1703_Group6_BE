using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("SyllabusScheduleTests")]
    public class SyllabusScheduleTest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(20)] // nếu đúng là string
        [ForeignKey("SyllabusSchedule")]
        public string SyllabusSchedulesID { get; set; }

        [Required]
        public TestCategory TestCategory { get; set; }

        [Required]
        public TestType TestType { get; set; }

        [Required]
        public virtual SyllabusSchedule SyllabusSchedule { get; set; }
    }
}
