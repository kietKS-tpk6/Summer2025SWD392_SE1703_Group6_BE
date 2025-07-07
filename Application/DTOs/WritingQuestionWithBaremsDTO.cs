using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class WritingQuestionWithBaremsDTO
    {
        public string QuestionID { get; set; }
        public string? Context { get; set; }
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
        public List<WritingBaremDTO> Barems { get; set; }
    }
}
