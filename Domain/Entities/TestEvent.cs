using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Domain.Enums;
namespace Domain.Entities
{
    [Table("TestEvents")]
    public class TestEvent
    {
        [Key]
        [MaxLength(6)]
        public string TestEventID { get; set; }

        [MaxLength(6)]
        [ForeignKey("Test")]
        public string TestID { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public int DurationMinutes { get; set; }

        public GradingMethod GradingMethod { get; set; }

        public TestCategory Category { get; set; }

        public TestEventStatus Status { get; set; }

        public virtual Test Test { get; set; }
    }
}
