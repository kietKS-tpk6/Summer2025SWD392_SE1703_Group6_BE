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
            public string? TestID { get; set; }

            public string? Description { get; set; }

            public DateTime? StartAt { get; set; }

            public DateTime? EndAt { get; set; }

            public int DurationMinutes { get; set; }

            [MaxLength(30)]
            public TestType TestType { get; set; }

            [MaxLength(15)]
            public TestEventStatus Status { get; set; }

            [MaxLength(6)]
            [ForeignKey("ScheduleTest")]
            public string? ScheduleTestID { get; set; }

            public int? AttemptLimit { get; set; }

            [MaxLength(50)]
            public string? Password { get; set; }
            [Required]
            [MaxLength(7)]
            [ForeignKey("Lesson")]
            public string ClassLessonID { get; set; }

            public virtual Test? Test { get; set; }
            public virtual Lesson Lesson { get; set; }
            public virtual SyllabusScheduleTest? ScheduleTest { get; set; }
        }
    }

