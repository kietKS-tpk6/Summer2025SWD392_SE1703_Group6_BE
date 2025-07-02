using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class QuestionMCQExcelDTO
    {
        public int QuestionNumber { get; set; } 
        public string Content { get; set; } 
        public string OptionA { get; set; } 
        public string OptionB { get; set; } 
        public string OptionC { get; set; } 
        public string OptionD { get; set; } 
        public string CorrectAnswer { get; set; }
    }
}
