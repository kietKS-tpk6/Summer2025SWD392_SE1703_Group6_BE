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

        public string SubjectName { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public SyllabusStatus Status { get; set; }

    }
}
