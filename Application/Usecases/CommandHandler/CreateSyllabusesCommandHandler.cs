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
            private readonly ISubjectService _subjectService;

            public CreateSyllabusesCommandHandler(ISyllabusesService syllabusesService, ISubjectService subjectService)
            {
            _syslabusesService = syllabusesService;
            _subjectService = subjectService;
            }
            public async Task<bool> Handle(CreateSyllabusesCommand req, CancellationToken cancellationToken)
            {
                if(!await _subjectService.SubjectExistsAsync(req.SubjectID))
                    throw new ArgumentNullException("Môn học không tồn tại.");
                if(await _syslabusesService.IsValidSyllabusStatusForSubjectAsync(req.SubjectID))
                    throw new ArgumentNullException("Môn học này đã được gán chương trình học.(Mỗi môn học chỉ một chương trình học khả dụng)");

            var res = await _syslabusesService.createSyllabuses(req);
                    return res;
            }

        }
    
}
