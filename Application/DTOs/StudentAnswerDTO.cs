using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class StudentAnswerDTO
    {
        public string QuestionID { get; set; }

        public List<string> Answers { get; set; }
    }
}
