using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class SubjectCreateClassDTO
    {
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public double MinAverageScoreToPass { get; set; }
        public SubjectStatus Status { get; set; }

    }
}
