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
        private readonly ISubjectService _subjectService;

        public UpdateSyllabusesCommandHandler(ISyllabusesService syllabusesService, ISubjectService subjectService)
        {
            _syslabusesService = syllabusesService;
            _subjectService = subjectService;
        }
        public async Task<string> Handle(UpdateSyllabusesCommand req, CancellationToken cancellationToken)
        {
            if (! await _syslabusesService.ExistsSyllabusAsync(req.SyllabusID))
                throw new ArgumentNullException("Chương trình học không tồn tại.");
            
            var res = await _syslabusesService.UpdateSyllabusesAsync(req);
            return res;
        }

    }
}
