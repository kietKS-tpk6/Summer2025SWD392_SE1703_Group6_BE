using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Usecases.Command
{
    public class LessonCreateFromScheduleCommand : IRequest<bool>
    {
        public string ClassId { get; set; }
        public TimeOnly StartHour { get; set; }
        public List<DayOfWeek> DaysOfWeek { get; set; }
    }
}
