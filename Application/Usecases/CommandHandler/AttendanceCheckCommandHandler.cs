using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Application.Usecases.Command;
using Application.Common.Constants;
using Application.IServices;
namespace Application.Usecases.CommandHandler
{
    public class AttendanceCheckCommandHandler : IRequestHandler<AttendanceCheckCommand, OperationResult<bool>>
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ILessonService _lessonService;
        public AttendanceCheckCommandHandler(IAttendanceService attendanceService, ILessonService lessonService)
        {
            _attendanceService = attendanceService;
            _lessonService = lessonService;
        }
        public async Task<OperationResult<bool>> Handle(AttendanceCheckCommand request, CancellationToken cancellationToken)
        {
            var lessonExists = await _lessonService.GetLessonDetailByLessonIDAsync(request.LessonId);
            if (!lessonExists.Success || lessonExists == null)
            {
                return OperationResult<bool>.Fail(OperationMessages.NotFound("tiết học"));
            }
            return await _attendanceService.CheckAttendanceAsync(request);
        }
    }
}
