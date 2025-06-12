using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class LessonCreateFromScheduleCommand : IRequest<OperationResult<string>>
    {
        public string ClassId { get; set; }
        public TimeOnly StartHour { get; set; }
        public List<DayOfWeek> DaysOfWeek { get; set; }
    }
}
