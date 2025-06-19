using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SyllabusScheduleTestItemDto
    {
        public string AssessmentCriteriaID { get; set; }
        public int Duration { get; set; }
        public Domain.Enums.TestType TestType { get; set; }
    }
}
