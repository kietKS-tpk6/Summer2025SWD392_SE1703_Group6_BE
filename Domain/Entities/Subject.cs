using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        [MaxLength(6)]
        public string SubjectID { get; set; }

        [MaxLength(40)]
        public string SubjectName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public SubjectStatus Status { get; set; }
        public bool IsActive { get; set; }  

        public DateTime CreateAt { get; set; }

        public double MinAverageScoreToPass { get; set; }
    }
}
