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
    [Table("Classes")]
    public class Class
    {
        [Key]
        [MaxLength(6)]
        public string ClassID { get; set; }

        [ForeignKey("Lecturer")]
        [MaxLength(6)]
        public string LecturerID { get; set; }

        [ForeignKey("Subject")]
        [MaxLength(6)]
        public string SubjectID { get; set; }

        [MaxLength(255)]
        public string ClassName { get; set; }

        public int MinStudentAcp { get; set; }
        public int MaxStudentAcp { get; set; }

        public decimal PriceOfClass { get; set; }

        public ClassStatus Status { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime TeachingStartTime { get; set; }

        public string ImageURL { get; set; }

        public virtual Account Lecturer { get; set; }

        public virtual Subject Subject { get; set; }
    }
}
