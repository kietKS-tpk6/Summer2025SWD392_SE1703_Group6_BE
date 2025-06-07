using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using FluentValidation;

namespace Application.Usecases.CommandHandler
{
    public class LessonCreateFromScheduleCommandHandler : IRequestHandler<LessonCreateFromScheduleCommand, bool>
    {
        private readonly ILessonService _lessonService;
        private readonly ISyllabusScheduleService _syllabusScheduleService;
        private readonly IClassService _classService;
        public LessonCreateFromScheduleCommandHandler(ILessonService lessonService, ISyllabusScheduleService syllabusScheduleService, IClassService classService)
        {
            _lessonService = lessonService;
            _syllabusScheduleService = syllabusScheduleService;
            _classService = classService;
        }
        public async Task<bool> Handle(LessonCreateFromScheduleCommand request, CancellationToken cancellationToken)
        {
            var ClassDTO = await _classService.GetClassCreateLessonDTOByIdAsync(request.ClassId);
            if(ClassDTO == null)
            {
                throw new ValidationException("Class không tìm thấy.");
            }
            var requiredDays = await _syllabusScheduleService.GetMaxSlotPerWeekAsync(ClassDTO.SyllabusID);
            if (request.DaysOfWeek.Count != requiredDays)
            {
                throw new ValidationException($"Không đủ thứ để gán cho tất cả các lịch học. Cần chọn đủ {requiredDays} thứ trong tuần.");
            }
            var schedules = await _syllabusScheduleService.GetPublishedSchedulesBySyllabusIdAsync(ClassDTO.SyllabusID);
            if (schedules == null || !schedules.Any())
            {
                throw new ValidationException("Không có lịch học nào trong syllabus.");
            }

            return await _lessonService.CreateLessonsFromSchedulesAsync(
                request.ClassId,
                ClassDTO.LecturerID,
                request.StartHour,
                request.DaysOfWeek,
                schedules
                );

        }
    }
}
