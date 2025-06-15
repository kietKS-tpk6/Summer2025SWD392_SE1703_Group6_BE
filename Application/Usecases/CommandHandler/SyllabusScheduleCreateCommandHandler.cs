using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.VisualBasic;

namespace Application.Usecases.CommandHandler
{
    public class SyllabusScheduleCreateCommandHandler : IRequestHandler<SyllabusScheduleCreateCommand, OperationResult<List<SyllabusScheduleWithSlotDto>>>
    {
        public const int LIMIT_SLOT_IN_WEEK = 5;

        private readonly ISyllabusScheduleService _syllabusScheduleService;
        private readonly ISubjectService _subjectService;

        public SyllabusScheduleCreateCommandHandler(ISyllabusScheduleService syllabusScheduleService, ISubjectService subjectService)
        {
            _syllabusScheduleService = syllabusScheduleService;
            _subjectService = subjectService;
        }
        public async Task<OperationResult<List<SyllabusScheduleWithSlotDto>>> Handle(SyllabusScheduleCreateCommand req, CancellationToken cancellationToken)
        {
            if (req.slotInWeek > LIMIT_SLOT_IN_WEEK)
            {
                return OperationResult<List<SyllabusScheduleWithSlotDto>>.Fail($"Không thể thêm quá {LIMIT_SLOT_IN_WEEK} slot cho cùng một tuần.");
            }
            var existsSyllabus = await _subjectService.SubjectExistsAsync(req.subjectID);
            if (!existsSyllabus)
            {
                return OperationResult<List<SyllabusScheduleWithSlotDto>>.Fail("SubjectID không tồn tại.");
            }

            var res = await _syllabusScheduleService.CreateEmptySyllabusScheduleAyncs(req);
            return res;
        }


    }
}
