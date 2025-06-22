using Domain.Enums;
using System;

namespace Application.DTOs
{
    public class TestResponseDTO
    {
        public string TestID { get; set; }
        public string CreateBy { get; set; }
        public string CreatedByName { get; set; }
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public TestStatus Status { get; set; }
        public TestCategory Category { get; set; }
        public TestType TestType { get; set; }
        public int TotalSections { get; set; }
    }
}