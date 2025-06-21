using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class SyllabusScheduleCreateCommandHandler : IRequestHandler<SyllabusScheduleCreateCommand, OperationResult<List<SyllabusScheduleWithSlotDto>>>
    {
        private readonly ISyllabusScheduleService _syllabusScheduleService;
        private readonly ISubjectService _subjectService;
        private readonly ISystemConfigService _configService;

        public SyllabusScheduleCreateCommandHandler(
            ISyllabusScheduleService syllabusScheduleService,
            ISubjectService subjectService,
            ISystemConfigService configService)
        {
            _syllabusScheduleService = syllabusScheduleService;
            _subjectService = subjectService;
            _configService = configService;
        }

        private async Task<int> GetMaxSlotPerWeekAsync()
        {
            var config = await _configService.GetConfig("class_maxSlot");

            if (!config.Success || config.Data == null || string.IsNullOrWhiteSpace(config.Data.Value))
                return 5; // fallback mặc định nếu không có cấu hình

            if (int.TryParse(config.Data.Value, out int value))
                return value;

            return 5; // fallback nếu không parse được
        }

        public async Task<OperationResult<List<SyllabusScheduleWithSlotDto>>> Handle(SyllabusScheduleCreateCommand req, CancellationToken cancellationToken)
        {
            var maxSlot = await GetMaxSlotPerWeekAsync();

            if (req.slotInWeek > maxSlot)
            {
                return OperationResult<List<SyllabusScheduleWithSlotDto>>.Fail($"Không thể thêm quá {maxSlot} slot cho cùng một tuần.");
            }

            var existsSyllabus = await _subjectService.SubjectExistsAsync(req.subjectID);
            if (!existsSyllabus)
            {
                return OperationResult<List<SyllabusScheduleWithSlotDto>>.Fail("SubjectID không tồn tại.");
            }

            return await _syllabusScheduleService.CreateEmptySyllabusScheduleAyncs(req);
        }
    }
}
