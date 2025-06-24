using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StudentTestGradingDTO
    {
        public string StudentTestID { get; set; }
        public double Score { get; set; }
        public string Status { get; set; }
    }
}
