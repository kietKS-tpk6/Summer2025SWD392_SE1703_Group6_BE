using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class AdvancedTestFilterRequest
    {
        public AssessmentCategory? Category { get; set; }
        public string? SubjectId { get; set; }
        public TestType? TestType { get; set; }
        public TestStatus? Status { get; set; }
    }

}
