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
    [Table("ClassEnrollments")]
    public class ClassEnrollment
    {
        [Key]
        [MaxLength(6)]
        public string ClassEnrollmentID { get; set; }

        [ForeignKey("Student")]
        [MaxLength(6)]
        public string StudentID { get; set; }

        [ForeignKey("Class")]
        [MaxLength(6)]
        public string ClassID { get; set; }

        public DateTime EnrolledDate { get; set; }

        public EnrollmentStatus Status { get; set; }

        public virtual Account Student { get; set; }

        public virtual Class Class { get; set; }
    }
}
