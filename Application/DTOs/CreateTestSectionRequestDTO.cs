using Domain.Enums;

namespace Application.DTOs
{
    public class CreateTestSectionRequestDTO
    {
        public string TestID { get; set; }
        public string Context { get; set; }
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
        public TestFormatType TestSectionType { get; set; }
        public decimal? Score { get; set; }
    }
}