using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class SectionAnswerDTO
    {
        public string SectionID { get; set; }

        public TestFormatType FormatType { get; set; } 

        public List<StudentAnswerDTO> Answers { get; set; }
    }
}
