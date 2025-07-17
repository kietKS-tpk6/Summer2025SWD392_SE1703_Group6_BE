using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class ClassInfoUpdateDTO
    {
        public List<int> DayOfWeeks { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public string LecturerId { get; set; }
        public string LecturerName { get; set; }
        public decimal PriceOfClass { get; set; }
        public int NumberStudentEnroll { get; set; }
        public int MinStudentAcp { get; set; }
        public int MaxStudentAcp { get;set; }
        public DateTime TeachingStartTime { get; set; }
        public TimeOnly LessonTime { get; set; }
        public string ImageURL { get; set; }
        public ClassStatus Status { get; set; }
   
    }
}
