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
    public class CheckLecturerFreeCommand : IRequest<OperationResult<List<AccountDTO>>>
    {
        public string SubjectID { get; set; }
        public DateTime DateStart { get; set; }
        public TimeOnly Time { get; set; }
        public List<DayOfWeek> dayOfWeeks { get; set; }
    }
}
