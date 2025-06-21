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
    [Table("Tests")]
    public class Test
    {
        [Key]
        [MaxLength(6)]
        public string TestID { get; set; }

        [MaxLength(6)]
        [ForeignKey("Account")]
        public string CreateBy { get; set; }

        [MaxLength(6)]
        [ForeignKey("Subject")]
        public string SubjectID { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public TestStatus Status { get; set; }
        public TestCategory Category { get; set; }
        public TestType TestType { get; set; }  
        public string TestName { get; set; }
        public virtual Account Account { get; set; }


        public virtual Subject Subject { get; set; }
    }
}
