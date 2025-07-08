using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ManagerDashboardOverviewDTO
    {
        public int TotalLecturers { get; set; }
        public int TotalSubjects { get; set; }
        public int ActiveClasses { get; set; }
        public decimal TotalRevenue { get; set; }
    }

}
