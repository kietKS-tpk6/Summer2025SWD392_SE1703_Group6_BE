using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class TaskDTO
    {
        public string TaskID { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime Deadline { get; set; }
        public string? Note { get; set; }
        public string? ResourcesURL { get; set; }
        public string? AssignedLecturerID { get; set; }
        public string? AssignedLecturerName { get; set; }
        public string? Status { get; set; }
        public bool IsOverdue => DateTime.Now > Deadline;
        public int DaysUntilDeadline => (Deadline - DateTime.Now).Days;
    }

    public class TaskCreateDTO
    {
        public string LecturerID { get; set; }
        public TaskType Type { get; set; }
        public string Content { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime Deadline { get; set; }
        public string? Note { get; set; }
        public string? ResourcesURL { get; set; }
    }

    public class TaskListDTO
    {
        public List<TaskDTO> Tasks { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}