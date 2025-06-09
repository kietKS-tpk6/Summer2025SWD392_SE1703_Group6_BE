using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Entities
{
    [Table("SyllabusScheduleTests")]
    public class SyllabusScheduleTest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(20)]
        [ForeignKey("SyllabusSchedule")]
        public string SyllabusSchedulesID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TestCategory { get; set; }

        [Required]
        [MaxLength(100)]
        public string TestType { get; set; }

        public virtual SyllabusSchedule SyllabusSchedule { get; set; }
    }
}