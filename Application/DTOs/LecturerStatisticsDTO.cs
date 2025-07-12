using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class LecturerStatisticsDTO
    {
        public string LecturerID { get; set; } = null!;
        public string LecturerName { get; set; } = null!;
        public int TotalClasses { get; set; }
        public int OngoingClasses { get; set; }
        public int OpenClasses { get; set; }
        public int CancelledClasses { get; set; }
        public int CompletedClasses { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
