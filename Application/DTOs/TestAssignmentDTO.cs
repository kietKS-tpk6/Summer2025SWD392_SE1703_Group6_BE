using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class TestAssignmentDTO
    {
        public string TestEventID { get; set; }
        public string TestID { get; set; }
        public TestType TestType { get; set; }
        public int DurationMinutes { get; set; }
        public List<TestSectionAssignmentDTO> Sections { get; set; }
    }
}
