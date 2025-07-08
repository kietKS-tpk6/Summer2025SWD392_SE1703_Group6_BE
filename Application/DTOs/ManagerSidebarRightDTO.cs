using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class ManagerSidebarRightDTO
    {
        public int TodayClasses { get; set; }
        public int TodayTests { get; set; }
        public List<EligibleClassForOpeningInfo> EligibleClassForOpening { get; set; }
        public List<ClassNearOpenButNotReadyDTO> ClassNearOpenButNotReady { get; set; }
        public List<TestEventMissingTestIdInfo> TestEventsNeedingTestID { get; set; }
    }

    public class EligibleClassForOpeningInfo
    {
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public int StudentCount { get; set; }
        public DateTime TeachingStartTime { get; set; }
    }

    public class ClassNearOpenButNotReadyDTO
    {
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public DateTime TeachingStartTime { get; set; }
        public int StudentCount { get; set; }
        public int MinStudentAcpt { get; set; }
    }

    public class TestEventMissingTestIdInfo
    {
        public string TestEventID { get; set; }
        public string SubjectName { get; set; } 
        public DateTime TimeLessonStart { get; set; }
        public AssessmentCategory Category { get; set; } 
    }

}
