using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class SyllabusScheduleCreateCommandHandler : IRequestHandler<SyllabusScheduleCreateCommand, bool>
    {
        private readonly ISyllabusScheduleService _syllabusScheduleService;
        private readonly ISyllabusesService _syllabusesService;

        public SyllabusScheduleCreateCommandHandler(ISyllabusScheduleService syllabusScheduleService, ISyllabusesService syllabusesService)
        {
            _syllabusScheduleService = syllabusScheduleService;
            _syllabusesService = syllabusesService;
        }
        public async Task<bool> Handle(SyllabusScheduleCreateCommand req, CancellationToken cancellationToken)
        {
            if (!await _syllabusScheduleService.IsMaxSlotInWeek(req.SyllabusID, req.Week))
            {
                throw new InvalidOperationException("Không thể thêm quá 7 slot cho cùng một tuần.");
            }
            var existsSyllabus = await _syllabusesService.ExistsSyllabusAsync(req.SyllabusID);
            if (!existsSyllabus) throw new ArgumentException("SyllabusID không tồn tại.");

            var res = await _syllabusScheduleService.CreateSyllabusScheduleAyncs(req);
            return res;
        }

    }
}
