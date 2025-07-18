using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StudentPerformanceInClassDTO
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public double AttendanceRate { get; set; } 
        public double AverageScore { get; set; } 
        public string Status { get; set; } 
        public int AbsentSessions { get; set; } 
    }
}
