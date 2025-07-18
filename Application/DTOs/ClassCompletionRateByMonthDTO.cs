using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ClassCompletionRateByMonthDTO
    {
        public string Month { get; set; } 
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int Total => Completed + Cancelled;
        public int Rate => Total == 0 ? 0 : (int)(100.0 * Completed / Total);
    }
}
