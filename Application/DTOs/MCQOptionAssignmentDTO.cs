using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MCQOptionAssignmentDTO
    {
        public string OptionID { get; set; }
        public string? Context { get; set; }
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
    }
}
