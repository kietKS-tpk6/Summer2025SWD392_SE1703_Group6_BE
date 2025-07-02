using System.Collections.Generic;

namespace Application.DTOs
{
    public class BatchUpdateResultDTO
    {
        public int UpdatedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}