using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("ScheduleWorks")]
    public class ScheduleWork
    {
        [Key]
        [MaxLength(6)]
        public string ScheduleWorkID { get; set; }

        [ForeignKey("WorkTask")]
        [MaxLength(6)]
        public string TaskID { get; set; }

        [ForeignKey("Account")]
        [MaxLength(6)]
        public string AccountID { get; set; }

        public virtual WorkTask WorkTask { get; set; }

        public virtual Account Account { get; set; }
    }
}
