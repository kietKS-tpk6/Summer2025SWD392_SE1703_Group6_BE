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
        private readonly ISubjectService _subjectService;

        public SyllabusScheduleCreateCommandHandler(ISyllabusScheduleService syllabusScheduleService, ISubjectService subjectService)
        {
            _syllabusScheduleService = syllabusScheduleService;
            _subjectService = subjectService;
        }
        public async Task<bool> Handle(SyllabusScheduleCreateCommand req, CancellationToken cancellationToken)
        {
            if (req.SlotInWeek>5)
            {
                throw new InvalidOperationException("Không thể thêm quá 5 slot cho cùng một tuần.");
            }
            var existsSyllabus = await _subjectService.SubjectExistsAsync(req.SubjectID);
             if (!existsSyllabus) throw new ArgumentException("SubjectID không tồn tại.");

            var res = await _syllabusScheduleService.CreateEmptySyllabusScheduleAyncs(req);
            return res;
        }

    }
}
