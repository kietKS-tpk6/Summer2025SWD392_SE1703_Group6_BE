using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Usecases.Command
{
    public class LessonUpdateCommand : IRequest<bool>
    {
        public string ClassLessonID { get; set; }
        public string ClassID { get; set; }
        public string SyllabusScheduleID { get; set; }
        public string LecturerID { get; set; }
        public DateTime StartTime { get; set; }

    }
}
