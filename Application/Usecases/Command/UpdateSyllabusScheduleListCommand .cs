using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateSyllabusScheduleListCommand : IRequest<OperationResult<bool>>
    {
        public string SubjectID { get; set; }
        public List<SyllabusScheduleUpdateItemDto> ScheduleItems { get; set; }
    }
}
