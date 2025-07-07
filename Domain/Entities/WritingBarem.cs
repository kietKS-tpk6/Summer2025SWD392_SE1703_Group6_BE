using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WritingBarem
    {
        [Key]
        [StringLength(6)]
        public string WritingBaremID { get; set; }

        [Required]
        [StringLength(8)]
        public string QuestionID { get; set; }

        [Required]
        [StringLength(250)]
        public string CriteriaName { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal MaxScore { get; set; }

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;


        public Question Question { get; set; }
    }
}
