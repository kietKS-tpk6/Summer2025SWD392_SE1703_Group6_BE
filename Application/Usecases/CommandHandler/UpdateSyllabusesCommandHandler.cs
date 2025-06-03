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
    internal class UpdateSyllabusesCommandHandler : IRequestHandler<UpdateSyllabusesCommand, string>
    {
        private readonly ISyllabusesService _syslabusesService;
        public UpdateSyllabusesCommandHandler(ISyllabusesService syllabusesService)
        {
            _syslabusesService = syllabusesService;
        }
        public async Task<string> Handle(UpdateSyllabusesCommand req, CancellationToken cancellationToken)
        {
            var res = await _syslabusesService.UpdateSyllabusesAsync(req);
            return res;
        }

    }
}
