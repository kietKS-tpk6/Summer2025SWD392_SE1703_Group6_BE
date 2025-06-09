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
    public class AddTestSchedulesToSlotsCommandHandler : IRequestHandler<AddTestSchedulesToSlotsCommand, bool>
    {
        private readonly ISyllabusScheduleTestService _syllabusScheduleTestService;
        public AddTestSchedulesToSlotsCommandHandler(ISyllabusScheduleTestService syllabusScheduleTestService)
        {
            _syllabusScheduleTestService = syllabusScheduleTestService;
        }
        public async Task<bool> Handle(AddTestSchedulesToSlotsCommand request, CancellationToken cancellationToken)
        {
            return await _syllabusScheduleTestService.AddTestToSyllabusAsync(request);
        }

    }
}
