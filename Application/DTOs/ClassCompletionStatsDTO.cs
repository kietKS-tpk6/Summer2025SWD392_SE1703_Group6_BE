using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ClassCompletionStatsDTO
    {
        public string ClassId { get; set; } 
        public string ClassName { get; set; } 
        public string SubjectName { get; set; } 
        public int TotalStudents { get; set; }
        public int CompletedStudents { get; set; }
        public double AverageAttendanceRate { get; set; } 
        public double AverageScore { get; set; }         
        public double CompletionRate { get; set; }      
    }
}
