using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Column(TypeName = "nvarchar(20)")]
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.InProgress;

        /// <summary>
        /// Helper property to get TaskType enum from string Type
        /// </summary>
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