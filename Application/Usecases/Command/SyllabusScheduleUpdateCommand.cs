using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Usecases.Command
{
    public class SyllabusScheduleUpdateCommand : IRequest<bool>
    {
        public List<SyllabusScheduleUpdateItem> Items { get; set; }
    }

    public class SyllabusScheduleUpdateItem
    {
        public string SyllabusScheduleID { get; set; }
        public string Content { get; set; }
        public string Resources { get; set; }
        public string LessonTitle { get; set; }
        public int DurationMinutes { get; set; }
        public bool HasTest { get; set; }
    }

}
