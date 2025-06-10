using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using Infrastructure.IRepositories;
using Domain.Entities;
using Application.DTOs;
namespace Infrastructure.Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        public LessonService(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }
        public async Task<bool> CreateLessonAsync(LessonCreateCommand request)
        {
            var numLesson = await _lessonRepository.CountAsync();
            string newLessonID = "L" + numLesson.ToString("D6");
            string randomGuid16 = Guid.NewGuid().ToString("N").Substring(0, 16);
            string prefix = "hangullearningsystem/";
            string baseUrl = "https://meet.jit.si/";
            string roomUrl = baseUrl + prefix + randomGuid16;
            var newLesson = new Lesson
            {
                ClassLessonID = newLessonID,
                ClassID = request.ClassID,
                LecturerID = request.LecturerID,
                SyllabusScheduleID = request.SyllabusScheduleID,
                StartTime = request.StartTime,
                LinkMeetURL = roomUrl
            };
            return await _lessonRepository.CreateAsync(newLesson);
        }
        public async Task<bool> UpdateLessonAsync(LessonUpdateCommand request)
        {
            var updateLesson = new Lesson
            {
                ClassLessonID = request.ClassLessonID,
                ClassID = request.ClassID,
                LecturerID = request.LecturerID,
                SyllabusScheduleID = request.SyllabusScheduleID,
                StartTime = request.StartTime,
            };
            return await _lessonRepository.UpdateAsync(updateLesson);
        }
        public async Task<List<LessonDTO>> GetLessonsByClassID(string classID)
        {
            var lessons = await _lessonRepository.GetLessonsByClassIDAsync(classID);

            var result = lessons.Select(lesson => new LessonDTO
            {
                ClassLessonID = lesson.ClassLessonID,
                ClassID = lesson.ClassID,
                LectureID = lesson.LecturerID,
                SyllabusScheduleID = lesson.SyllabusScheduleID,
                StartTime = lesson.StartTime,
                EndTime = lesson.StartTime.AddMinutes(lesson.SyllabusSchedule?.DurationMinutes ?? 0),
                LinkMeetURL = lesson.LinkMeetURL,
                SubjectName = lesson.Class?.Subject?.SubjectName
            }).ToList();

            return result;
        }
        public async Task<List<LessonDTO>> GetLessonsByStudentID(string studentID)
        {
            var lessons = await _lessonRepository.GetLessonsByStudentIDAsync(studentID);

            var result = lessons.Select(lesson => new LessonDTO
            {
                ClassLessonID = lesson.ClassLessonID,
                ClassID = lesson.ClassID,
                LectureID = lesson.LecturerID,
                SyllabusScheduleID = lesson.SyllabusScheduleID,
                StartTime = lesson.StartTime,
                EndTime = lesson.StartTime.AddMinutes(lesson.SyllabusSchedule?.DurationMinutes ?? 0),
                LinkMeetURL = lesson.LinkMeetURL,
                SubjectName = lesson.Class?.Subject?.SubjectName
            }).ToList();

            return result;
        }
        public async Task<List<LessonDTO>> GetLessonsByLecturerID(string lecturerID)
        {
            var lessons = await _lessonRepository.GetLessonsByLecturerIDAsync(lecturerID);

            return lessons.Select(lesson => new LessonDTO
            {
                ClassLessonID = lesson.ClassLessonID,
                ClassID = lesson.ClassID,
                LectureID = lesson.LecturerID,
                SyllabusScheduleID = lesson.SyllabusScheduleID,
                StartTime = lesson.StartTime,
                EndTime = lesson.StartTime.AddMinutes(lesson.SyllabusSchedule?.DurationMinutes ?? 0),
                LinkMeetURL = lesson.LinkMeetURL,
                SubjectName = lesson.Class?.Subject?.SubjectName
            }).ToList();
        }
        public async Task<bool> DeleteLessonAsync(string classLessonID)
        {
            return await _lessonRepository.DeleteAsync(classLessonID);
        }
        public async Task<LessonDetailDTO> GetLessonDetailByLessonIDAsync(string classLessonID)
        {
            return await _lessonRepository.GetLessonDetailByLessonIDAsync(classLessonID);
        }
        private DateTime GetDateByWeekAndDay(DateTime startDate, int week, DayOfWeek day)
        {
            var baseMonday = startDate.AddDays(-(int)startDate.DayOfWeek + (int)DayOfWeek.Monday);

            var targetDate = baseMonday.AddDays(7 * (week - 1) + (int)day);
            return targetDate;
        }

        public async Task<bool> CreateLessonsFromSchedulesAsync(
              string classId,
              string lecturerId,
              TimeOnly startHour,
              List<DayOfWeek> selectedDays,
              List<SyllabusScheduleCreateLessonDTO> schedules,
              DateTime StartTime
        )
        {
            var lessonsToCreate = new List<Lesson>();
            var startDate = StartTime;
            int currentScheduleIndex = 0;
            int currentWeek = schedules.Min(s => s.Week);
            var totalSchedules = schedules.Count;

            while (currentScheduleIndex < totalSchedules)
            {
                foreach (var day in selectedDays)
                {
                    if (currentScheduleIndex >= totalSchedules) break;

                    var schedule = schedules[currentScheduleIndex];
                    DateTime targetDate;
                    if (currentScheduleIndex == 0)
                    {
                        targetDate = StartTime.Date;
                    }
                    else
                    {
                        targetDate = GetDateByWeekAndDay(startDate, schedule.Week, day);
                    }

                    var numLesson = await _lessonRepository.CountAsync();
                    string newLessonID = "L" + (numLesson + lessonsToCreate.Count).ToString("D6");
                    string roomUrl = "https://meet.jit.si/hangullearningsystem/" + Guid.NewGuid().ToString("N").Substring(0, 16);

                    lessonsToCreate.Add(new Lesson
                    {
                        ClassLessonID = newLessonID,
                        ClassID = classId,
                        LecturerID = lecturerId,
                        SyllabusScheduleID = schedule.SyllabusScheduleId,
                        StartTime = currentScheduleIndex == 0
                                    ? StartTime
                                    : targetDate.Add(startHour.ToTimeSpan()),
                        LinkMeetURL = roomUrl,
                        IsActive = true
                    });

                    currentScheduleIndex++;
                }
            }

            return await _lessonRepository.CreateManyAsync(lessonsToCreate);
        }

    }
}
