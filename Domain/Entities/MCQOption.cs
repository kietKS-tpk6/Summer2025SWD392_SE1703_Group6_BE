using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("MCQOptions")]
    public class MCQOption
    {
        [Key]
        [MaxLength(8)]
        public string MCQOptionID { get; set; }

        [MaxLength(8)]
        [ForeignKey("Question")]
        public string QuestionID { get; set; }

        [MaxLength(255)]
        public string? Context { get; set; }

        public string? ImageURL { get; set; }

        public string? AudioURL { get; set; }

        public bool IsCorrect { get; set; }

        public virtual Question Question { get; set; }
    }
}
