using System;
using Domain.Enums;

namespace Application.DTOs
{
    public class SubjectDTO
    {
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public SubjectStatus Status { get; set; }
        public DateTime CreateAt { get; set; }
        public double MinAverageScoreToPass { get; set; }
    }
}