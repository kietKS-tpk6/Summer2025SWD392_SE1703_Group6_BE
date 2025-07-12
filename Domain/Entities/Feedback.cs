using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Feedback")]
    public class Feedback
    {
        [Key]
        [MaxLength(6)]
        public string FeedbackID { get; set; }

        [MaxLength(6)]
        [ForeignKey("Class")]
        public string ClassID { get; set; }

        [MaxLength(6)]
        [ForeignKey("Student")]
        public string StudentID { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime FeedbackAt { get; set; }

        public virtual Class Class { get; set; }

        public virtual Account Student { get; set; }
    }
}
