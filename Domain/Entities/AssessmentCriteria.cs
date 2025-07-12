using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("AssessmentCriteria")]
    public class AssessmentCriteria
    {
        [Key]
        [MaxLength(6)]
        public string AssessmentCriteriaID { get; set; }

        [ForeignKey("Subject")]
        [MaxLength(6)]
        public string SubjectID { get; set; }
        public double? WeightPercent { get; set; }

        public AssessmentCategory? Category { get; set; }

        public int? RequiredTestCount { get; set; }

        [MaxLength(255)]
        public string? Note { get; set; }
        public bool IsActive { get; set; }
        public decimal? MinPassingScore { get; set; }

        public virtual Subject Subject { get; set; }
    }
}
