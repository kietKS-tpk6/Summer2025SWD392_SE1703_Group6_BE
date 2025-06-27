using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class AttendanceRecordDTO
    {
        public List<LessonAttendanceDTO> LessonAttendances { get; set; }
    }
    public class LessonAttendanceDTO
    {
        public string LessonID { get; set; }
        public string LessonTitle { get; set; }
        public List<StudentAttendanceRecordDTO> StudentAttendanceRecords { get; set; }
    }
    public class StudentAttendanceRecordDTO
    {
        public string AttendanceRecordID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public string? Note { get; set; }
    }
}
