using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace Domain.Entities
    {
        [Table("MCQAnswerDetails")]
        public class MCQAnswerDetail
        {
            [Key]
            [MaxLength(8)]
            public string MCQAnswerDetailID { get; set; }

            [Required]
            [MaxLength(6)]
            [ForeignKey("MCQAnswer")]
            public string MCQAnswerID { get; set; }

            [Required]
            [MaxLength(8)]
            [ForeignKey("MCQOption")]
            public string MCQOptionID { get; set; }

            // Navigation properties
            public virtual MCQAnswer MCQAnswer { get; set; }
            public virtual MCQOption MCQOption { get; set; }
        }
    }

