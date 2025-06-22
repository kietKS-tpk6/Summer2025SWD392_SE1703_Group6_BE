using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class SyllabusScheduleCreateCommand : IRequest<OperationResult<List<SyllabusScheduleWithSlotDTO>>>
    {
        
        public string subjectID { get; set; }

        public int week { get; set; }
        public int slotInWeek { get; set; }

//public string Content {  get; set; }
        //public string SubjectID { get; set; }
        //public int Week { get; set; }
        //public string Resources { get; set; }
        //public string LessonTitle { get; set; }
        //public int DurationMinutes { get; set; }
        //public bool HasTest { get; set; }



    }
}
