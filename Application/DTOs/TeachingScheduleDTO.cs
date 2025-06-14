using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TeachingScheduleDTO
    {
        public string LecturerID { get; set; }
        public string LecturerName { get; set; }
        public int TeachingDay { get; set; } 
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
