using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class LessonDetailDTO
    {
        //Lesson
        public string ClassLessonID { get; set; }
        public string LecturerID { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LinkMeetURL { get; set; }
        //Class
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        //ClassEnrollments
        public int NumberStudentEnroll { get; set; }
        //Subject
        public string SubjectName { get; set; }
        //Account
        public string LecturerName { get; set; }
        //SyllabusSchedule 
        public string SyllabusScheduleID { get; set; }
        public bool HasTest { get; set; }
        public string LessonTitle { get; set; }
        public string Content { get; set; }
        public string Resources { get; set; }
    }
}
