using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using FluentValidation;
using Application.Common.Constants;
using Application.DTOs;

namespace Application.Usecases.CommandHandler
{
    public class LessonCreateFromScheduleCommandHandler : IRequestHandler<LessonCreateFromScheduleCommand, OperationResult<string>>
    {
        private readonly ILessonService _lessonService;
        private readonly ISyllabusScheduleService _syllabusScheduleService;
        private readonly IClassService _classService;
        private readonly IAccountService _accountService;
        public LessonCreateFromScheduleCommandHandler(ILessonService lessonService, ISyllabusScheduleService syllabusScheduleService, IClassService classService, IAccountService accountService)
        {
            _lessonService = lessonService;
            _syllabusScheduleService = syllabusScheduleService;
            _classService = classService;
            _accountService = accountService;
        }
        public async Task<OperationResult<string>> Handle(LessonCreateFromScheduleCommand request, CancellationToken cancellationToken)
        {
            var classResult = await _classService.GetClassCreateLessonDTOByIdAsync(request.ClassId);
            if (!classResult.Success || classResult.Data == null)
            {
                return OperationResult<string>.Fail(classResult.Message ?? "Không tìm thấy lớp học.");
            }

            var classData = classResult.Data;

            var requiredDaysResult = await _syllabusScheduleService.GetMaxSlotPerWeekAsync(classData.SubjectId);
            if (!requiredDaysResult.Success || requiredDaysResult.Data <= 0)
            {
                return OperationResult<string>.Fail(requiredDaysResult.Message ?? "Không thể lấy số tiết tối đa mỗi tuần.");
            }

            int requiredDays = requiredDaysResult.Data;
            if (request.DaysOfWeek.Count != requiredDays)
            {
                return OperationResult<string>.Fail($"Không đủ số ngày trong tuần để gán lịch học. Cần chọn chính xác {requiredDays} ngày trong tuần.");
            }

            var isLecturerFreeResult = await _accountService.IsLectureFreeAsync(
                classData.LecturerID,
                classData.SubjectId,
                request.StartHour,
                request.DaysOfWeek
            );

            if (!isLecturerFreeResult.Success || !isLecturerFreeResult.Data)
            {
                return OperationResult<string>.Fail(isLecturerFreeResult.Message ?? "Giảng viên bị trùng lịch dạy với thời gian này. Vui lòng chọn thời gian khác.");
            }

            var schedulesResult = await _syllabusScheduleService.GetSchedulesBySubjectIdAsync(classData.SubjectId);
            if (!schedulesResult.Success || schedulesResult.Data == null || !schedulesResult.Data.Any())
            {
                return OperationResult<string>.Fail(schedulesResult.Message ?? "Không có lịch học nào trong giáo trình.");
            }

            var schedules = schedulesResult.Data;
            for (int i = 0; i < schedules.Count; i++)
            {
                schedules[i].DayOfWeek = request.DaysOfWeek[i % request.DaysOfWeek.Count];
            }

            var createResult = await _lessonService.CreateLessonsFromSchedulesAsync(
                request.ClassId,
                classData.LecturerID,
                request.StartHour,
                request.DaysOfWeek,
                schedules,
                classData.StartTime
            );

            if (!createResult.Success || !createResult.Data)
            {
                return OperationResult<string>.Fail(createResult.Message ?? OperationMessages.CreateFail("tiết học"));
            }

            return OperationResult<string>.Ok(
                OperationMessages.CreateSuccess("tiết học"),
                OperationMessages.CreateSuccess("tiết học")
            );

        }

    }
}
