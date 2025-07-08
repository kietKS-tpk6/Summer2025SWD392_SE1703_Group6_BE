using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UpdateTaskStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? LecturerID { get; set; }
        public DateTime? DateStart { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
