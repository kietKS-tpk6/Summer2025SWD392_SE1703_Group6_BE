using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class TestSectionAssignmentDTO

    {
        
    public string TestSectionID { get; set; }
    public TestFormatType FormatType { get; set; }
    public string? Context { get; set; }
    public string? ImageURL { get; set; }
    public string? AudioURL { get; set; }
    public decimal Score { get; set; }
    public List<QuestionAssignmentDTO> Questions { get; set; }
}
}
