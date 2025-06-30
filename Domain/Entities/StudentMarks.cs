using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{

    [Table("StudentMarks")]
    public class StudentMark
    {
        [Key]
        [Column("StudentMarkID")]
        public string StudentMarkID { get; set; }

        [Column("AccountID")]
        [StringLength(6)]
        public string AccountID { get; set; }

        [Column("AssessmentCriteriaID")]
        [StringLength(6)]
        public string AssessmentCriteriaID { get; set; }

        [Column("Mark")]
        public decimal? Mark { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string? Comment { get; set; }

        [Column("GradedBy")]
        [StringLength(6)]
        public string? GradedBy { get; set; }

        [Column("GradedAt")]
        public DateTime? GradedAt { get; set; }

        [Column("IsFinalized")]
        public bool IsFinalized { get; set; } = false;

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("StudentTestID")]
        [StringLength(6)]
        public string? StudentTestID { get; set; }

        [Column("ClassID")]
        [StringLength(6)]
        public string? ClassID { get; set; }

        // Navigation Properties
        [ForeignKey("AccountID")]
        public virtual Account? Account { get; set; }

        [ForeignKey("GradedBy")]
        public virtual Account? GradedByAccount { get; set; }

        [ForeignKey("StudentTestID")]
        public virtual StudentTest? StudentTest { get; set; }

        [ForeignKey("ClassID")]
        public virtual Class? Class { get; set; }
    }
}
