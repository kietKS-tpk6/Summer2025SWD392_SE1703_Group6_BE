using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AssessmentCompletenessResultDTO
    {
        public string Message { get; set; } = string.Empty;
        public List<MissingTestDTO> MissingTests { get; set; } = new List<MissingTestDTO>();
    }
}
