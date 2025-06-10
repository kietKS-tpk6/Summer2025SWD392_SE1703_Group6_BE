using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ClassCreateLessonDTO
    {
        public string SubjectId { get; set; }
        public string LecturerID { get; set; }
        public string SyllabusID { get; set; }
        public DateTime StartTime { get; set; }
    }
}
