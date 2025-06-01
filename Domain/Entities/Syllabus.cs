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
    [Table("Syllabuses")]
    public class Syllabus
    {
        [Key]
        [MaxLength(6)]
        public string SyllabusID { get; set; }

        [MaxLength(6)]
        public string SubjectID { get; set; }

        [MaxLength(6)]
        [ForeignKey("Creator")]
        public string CreateBy { get; set; }

        public DateTime CreateAt { get; set; }

        [MaxLength(6)]
        [ForeignKey("Updater")]
        public string? UpdateBy { get; set; }

        public DateTime? UpdateAt { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Note { get; set; }

        public SyllabusStatus Status { get; set; }

        public virtual Account Creator { get; set; }  // Account có Role Manager

        public virtual Account? Updater { get; set; }
    }
}
