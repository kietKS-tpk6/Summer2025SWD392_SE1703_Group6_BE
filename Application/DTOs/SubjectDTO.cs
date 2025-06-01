using System;

namespace Application.DTOs
{
    public class SubjectDTO
    {
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public float MinAverageScoreToPass { get; set; }
    }

    public class CreateSubjectDTO
    {
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public float MinAverageScoreToPass { get; set; } = 5.0f;
    }

    public class UpdateSubjectDTO
    {
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public float MinAverageScoreToPass { get; set; }
    }
}