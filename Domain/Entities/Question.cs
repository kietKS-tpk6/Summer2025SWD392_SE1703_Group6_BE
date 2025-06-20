using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("Questions")]
    public class Question
    {
        [Key]
        [MaxLength(8)]
        public string QuestionID { get; set; }

        [MaxLength(6)]
        [ForeignKey("TestSection")]
        public string TestSectionID { get; set; }

        public float Score { get; set; }

        public string Context { get; set; }

        public string ImageURL { get; set; }

        public string AudioURL { get; set; }

        public TestFormatType Type { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual TestSection TestSection { get; set; }
    }
}
