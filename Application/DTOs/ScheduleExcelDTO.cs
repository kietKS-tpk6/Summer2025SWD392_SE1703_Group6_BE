using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ScheduleExcelDTO
    {
        public int Week { get; set; }
        public int Slot { get; set; }
        public string Title { get; set; } 
        public string Content { get; set; } 
        public int DurationMinutes { get; set; }
        public string ResourceUrl { get; set; }
    }

}
