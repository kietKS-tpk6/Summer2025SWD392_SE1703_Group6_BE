using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("Tasks")]
    public class WorkTask
    {
        [Key]
        [MaxLength(6)]
        public string TaskID { get; set; }

        [MaxLength(30)]
        public string Type { get; set; }

        [MaxLength(255)]
        public string Content { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime Deadline { get; set; }

        [MaxLength(255)]
        public string? Note { get; set; }

        public string? ResourcesURL { get; set; }

        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.InProgress;

        [JsonIgnore]
        public virtual ICollection<ScheduleWork> ScheduleWorks { get; set; } = new List<ScheduleWork>();

        [NotMapped]
        public string? AccountID => ScheduleWorks?.FirstOrDefault()?.AccountID;

        [NotMapped]
        public TaskType TaskType
        {
            get
            {
                if (Enum.TryParse<TaskType>(Type, out var taskType))
                {
                    return taskType;
                }
                return TaskType.Other;
            }
        }

        [NotMapped]
        public bool RequiresManualCompletion => TaskType.RequiresManualCompletion();

        [NotMapped]
        public bool ShouldAutoCompleteOnDeadline => !RequiresManualCompletion;

        [NotMapped]
        public bool IsDeadlinePassed => DateTime.Now >= Deadline;

        [NotMapped]
        public bool ShouldAutoCompleteNow => ShouldAutoCompleteOnDeadline && IsDeadlinePassed && Status == Enums.TaskStatus.InProgress;
    }
}