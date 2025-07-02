using System.Collections.Generic;

namespace Application.DTOs
{
    public class GetTestScoresDTO
    {
        public string TestId { get; set; }
        public List<TestSectionScoreDTO> TestSections { get; set; }
        public decimal TotalScore { get; set; }
    }
}