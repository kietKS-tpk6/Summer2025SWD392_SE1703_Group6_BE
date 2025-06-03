using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class ClassDTO
    {
        public string ClassID { get; set; }
        public string LecturerID { get; set; }
        public string SubjectID { get; set; }
        public string ClassName { get; set; }
        public int MinStudentAcp { get; set; }
        public int MaxStudentAcp { get; set; }
        public decimal PriceOfClass { get; set; }
        public ClassStatus Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime TeachingStartTime { get; set; }
        public string ImageURL { get; set; }
        public string LecturerName { get; set; }
        public string SubjectName { get; set; }
    }

}
