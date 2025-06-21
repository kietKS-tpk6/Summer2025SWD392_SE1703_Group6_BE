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
    [Table("TestSections")]
    public class TestSection
    {
        [Key]
        [MaxLength(6)]
        public string TestSectionID { get; set; }

        [MaxLength(6)]
        [ForeignKey("Test")]
        public string TestID { get; set; }

        [MaxLength(255)]
        public string Context { get; set; }

        public string ImageURL { get; set; }

        [MaxLength(255)]
        public string AudioURL { get; set; }

        public TestFormatType TestSectionType { get; set; }

        public decimal Score { get; set; }

        public Boolean IsActive { get; set; } = false;

        public virtual Test Test { get; set; }
    }
}
