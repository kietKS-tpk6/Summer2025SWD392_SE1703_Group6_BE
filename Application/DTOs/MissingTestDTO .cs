using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MissingTestDTO
    {
        public string Category { get; set; } = string.Empty;
        public string TestType { get; set; } = string.Empty;
        public int MissingCount { get; set; }
    }
}
