using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class QuestionDetailDto
    {
        public string QuestionID { get; set; }
        public string? Context { get; set; }
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
        public TestFormatType Type { get; set; }
        public decimal Score { get; set; }
        public bool IsActive { get; set; }
        public List<MCQOptionDto>? Options { get; set; }
    }
}
