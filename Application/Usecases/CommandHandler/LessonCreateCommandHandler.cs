using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class LessonCreateCommandHandler : IRequestHandler<LessonCreateCommand, OperationResult<bool>>
    {
        private readonly ILessonService _lessonService;
        public LessonCreateCommandHandler(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }
        public async Task<OperationResult<bool>> Handle(LessonCreateCommand request, CancellationToken cancellationToken)
        {
            return await _lessonService.CreateLessonAsync(request);
        }

    }
}
