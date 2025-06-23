using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class LessonContentDTO
    {
        public string ClassLessonID { get; set; }
        public string LectureID { get; set; }
        public string SyllabusScheduleID { get; set; }
        public string LessonTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LinkMeetURL { get; set; }
        public string SubjectName { get; set; }
        public string LectureName { get; set; }
    }
}
