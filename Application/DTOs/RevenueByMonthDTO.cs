using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RevenueByMonthDTO
    {
        public string Month { get; set; } // Format: MM/yyyy
        public decimal Revenue { get; set; }
    }

}
