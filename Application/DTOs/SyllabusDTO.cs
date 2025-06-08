using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class SyllabusDTO
    {
        public string SyllabusID { get; set; }
        public string SubjectID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateAt { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Status { get; set; } // Enum dưới dạng string

    }
}
