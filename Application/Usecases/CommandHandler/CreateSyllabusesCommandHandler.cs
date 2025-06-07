using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    
        public class CreateSyllabusesCommandHandler : IRequestHandler<CreateSyllabusesCommand, bool>
        {
            private readonly ISyllabusesService _syslabusesService;
            public CreateSyllabusesCommandHandler(ISyllabusesService syllabusesService)
            {
            _syslabusesService = syllabusesService;
            }
            public async Task<bool> Handle(CreateSyllabusesCommand req, CancellationToken cancellationToken)
            {
                var res = await _syslabusesService.createSyllabuses(req);
                return res;
            }

        }
    
}
