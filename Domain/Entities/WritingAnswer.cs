using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("WritingAnswers")]
    public class WritingAnswer
    {
        [Key]
        [MaxLength(6)]
        public string WritingAnswerID { get; set; }

        [MaxLength(6)]
        [ForeignKey("StudentTest")]
        public string StudentTestID { get; set; }

        [MaxLength(8)]
        [ForeignKey("Question")]
        public string QuestionID { get; set; }

        public string StudentEssay { get; set; }

        public string? Feedback { get; set; }

        public decimal? Score { get; set; }

        // Navigation properties
        public virtual StudentTest StudentTest { get; set; }
        public virtual Question Question { get; set; }
    }
}
