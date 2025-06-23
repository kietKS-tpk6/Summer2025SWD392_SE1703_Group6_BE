using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class LessonUpdateCommand : IRequest<OperationResult<bool>>
    {
        public string ClassLessonID { get; set; }
        public string LecturerID { get; set; }
        public DateTime StartTime { get; set; }

    }
}
