﻿using System;
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
    public class LessonUpdateCommandHandler : IRequestHandler<LessonUpdateCommand, OperationResult<bool>>
    {
        private readonly ILessonService _lessonService;
        public LessonUpdateCommandHandler(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }
        public async Task<OperationResult<bool>> Handle(LessonUpdateCommand request, CancellationToken cancellationToken)
        {
            return await _lessonService.UpdateLessonAsync(request);
        }

    }
}
