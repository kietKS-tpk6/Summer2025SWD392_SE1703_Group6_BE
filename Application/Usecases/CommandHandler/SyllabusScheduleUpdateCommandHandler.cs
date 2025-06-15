using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class SyllabusScheduleUpdateCommandHandler : IRequestHandler<SyllabusScheduleUpdateCommand, OperationResult<bool>>
    {

        private readonly ISyllabusScheduleService _syllabusScheduleService;
        private readonly ISubjectService _subjectService;

        public SyllabusScheduleUpdateCommandHandler(ISyllabusScheduleService syllabusScheduleService, ISubjectService subjectService)
        {
            _syllabusScheduleService = syllabusScheduleService;
            _subjectService = subjectService;
        }
        public async Task<OperationResult<bool>> Handle(SyllabusScheduleUpdateCommand req, CancellationToken cancellationToken)
        {
            var res = await _syllabusScheduleService.UpdateSyllabusSchedulesAsync(req);
            if (res)
            {
                return OperationResult<bool>.Ok(true);
            }
            else
            {
                return OperationResult<bool>.Fail("Cập nhật SyllabusSchedule thất bại.");
            }
        }

    }
}
