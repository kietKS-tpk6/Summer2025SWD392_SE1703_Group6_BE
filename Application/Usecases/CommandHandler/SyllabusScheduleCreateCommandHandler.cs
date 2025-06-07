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
        public SyllabusScheduleCreateCommandHandler(ISyllabusScheduleService syllabusScheduleService)
        {
            _syllabusScheduleService = syllabusScheduleService;
        }
        public async Task<bool> Handle(SyllabusScheduleCreateCommand req, CancellationToken cancellationToken)
        {
            var res = await _syllabusScheduleService.CreateSyllabusScheduleAyncs(req);
            return res;
        }
    }
}
