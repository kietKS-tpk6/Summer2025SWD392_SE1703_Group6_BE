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

namespace Application.Usecases.CommandHandler
{
    public class LessonCreateFromScheduleCommandHandler : IRequestHandler<LessonCreateFromScheduleCommand, string>
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
        public async Task<string> Handle(LessonCreateFromScheduleCommand request, CancellationToken cancellationToken)
        {
            //var ClassDTO = await _classService.GetClassCreateLessonDTOByIdAsync(request.ClassId);
            //if (ClassDTO == null)
            //{
            //    return "Không tìm thấy lớp học.";
            //}

            //var requiredDays = await _syllabusScheduleService.GetMaxSlotPerWeekAsync(ClassDTO.SyllabusID);
            //if (request.DaysOfWeek.Count != requiredDays)
            //{
            //    return $"Không đủ thứ để gán cho tất cả lịch học. Cần chọn đủ {requiredDays} thứ trong tuần.";
            //}

            //var schedules = await _syllabusScheduleService.GetPublishedSchedulesBySyllabusIdAsync(ClassDTO.SyllabusID);
            //if (schedules == null || !schedules.Any())
            //{
            //    return "Không có lịch học nào trong giáo trình.";
            //}

            //var isLecturerFree = await _accountService.IsLectureFree(ClassDTO.LecturerID, request.StartHour, request.DaysOfWeek);
            //if (!isLecturerFree)
            //{
            //    return "Giảng viên bị trùng lịch dạy với thời gian này. Vui lòng chọn thời gian khác.";
            //}

            //var result = await _lessonService.CreateLessonsFromSchedulesAsync(
            //    request.ClassId,
            //    ClassDTO.LecturerID,
            //    request.StartHour,
            //    request.DaysOfWeek,
            //    schedules,
            //    ClassDTO.StartTime
            //);

            
           // return result ? OperationMessages.CreateSuccess : OperationMessages.CreateFail;
            return "sua lại hàm";
        }
    }
}
