using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public static class TaskTypeExtensions
    {
        public static bool RequiresManualCompletion(this TaskType taskType)
        {
            return taskType switch
            {
                TaskType.GradeAssignment => true,
                TaskType.CreateExam => true,
                TaskType.UpdateTestEvent => true,
                TaskType.Meeting => false,
                TaskType.ReviewContent => true,
                TaskType.PrepareLesson => true,
                TaskType.Other => false,
                _ => true
            };
        }

        public static List<TaskType> GetAutoCompleteTaskTypes()
        {
            return new List<TaskType>
            {
                TaskType.Meeting,
                TaskType.Other
            };
        }
    }
}