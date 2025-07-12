using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Domain.Entities
{
    [Table("MCQAnswers")]
    public class MCQAnswer
    {
        [Key]
        [MaxLength(6)]
        public string MCQAnswerID { get; set; }

        [Required]
        [MaxLength(6)]
        [ForeignKey("StudentTest")]
        public string StudentTestID { get; set; }

        [Required]
        [MaxLength(8)]
        [ForeignKey("Question")]
        public string QuestionID { get; set; }

        // Navigation properties
        public virtual StudentTest StudentTest { get; set; }
        public virtual Question Question { get; set; }
    }
}
