using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class QuestionAssignmentDTO
    {
        public string QuestionID { get; set; }
        public string? Content { get; set; }
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
        public int? NumberOfCorrectAnswers { get; set; }
        public List<MCQOptionAssignmentDTO>? Options { get; set; }

    }
}
