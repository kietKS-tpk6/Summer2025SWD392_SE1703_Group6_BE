using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class LessonDetailDTO
    {
        public string ClassLessonID { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string LecturerName { get; set; }
        public string LecturerID { get; set; }
        public string LessonTitle { get; set; }
        public string Content { get; set; }
        public string Resources { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
