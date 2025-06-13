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
    public class SyllabusScheduleUpdateCommandHandler : IRequestHandler<SyllabusScheduleUpdateCommand, bool>
    {

        private readonly ISyllabusScheduleService _syllabusScheduleService;
        private readonly ISubjectService _subjectService;

        public SyllabusScheduleUpdateCommandHandler(ISyllabusScheduleService syllabusScheduleService, ISubjectService subjectService)
        {
            _syllabusScheduleService = syllabusScheduleService;
            _subjectService = subjectService;
        }
        public async Task<bool> Handle(SyllabusScheduleUpdateCommand req, CancellationToken cancellationToken)
        {
           
            //check danh sach slot do co ton tai hay k
            var res = await _syllabusScheduleService.CheckListSyllabusScheduleAsync(req.Items);
            if(res == false)
            {
                return false;
            }

            return await _syllabusScheduleService.UpdateSyllabusSchedulesAsync(req);

        }
    }
}
