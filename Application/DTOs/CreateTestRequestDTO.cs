using Domain.Enums;

namespace Application.DTOs
{
    public class CreateTestRequestDTO
    {
        public string SubjectID { get; set; }
        public TestType TestType { get; set; }
        public TestCategory Category { get; set; }
    }
}