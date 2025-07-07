using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateWritingBaremDTO
    {
        public string QuestionID { get; set; }
        public string CriteriaName { get; set; }
        public decimal MaxScore { get; set; }
        public string? Description { get; set; }
    }

}
