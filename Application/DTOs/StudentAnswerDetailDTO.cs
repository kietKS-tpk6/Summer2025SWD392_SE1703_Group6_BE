using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StudentAnswerDetailDTO
    {
        public string? MCQAnswerID { get; set; }
        public List<string>? SelectedOptionIDs { get; set; } // Danh sách MCQOptionID được chọn
        public string? WritingAnswerID { get; set; }
        public string? StudentEssay { get; set; }
        public string? Feedback { get; set; }
        public decimal? WritingScore { get; set; }
    }
}
