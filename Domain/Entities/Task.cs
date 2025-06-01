using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Tasks")]
    public class Task
    {
        [Key]
        [MaxLength(6)]
        public string TaskID { get; set; }

        [MaxLength(30)]
        public string Type { get; set; }

        [MaxLength(255)]
        public string Content { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime Deadline { get; set; }

        [MaxLength(255)]
        public string? Note { get; set; }

        public string? ResourcesURL { get; set; }
    }
}
