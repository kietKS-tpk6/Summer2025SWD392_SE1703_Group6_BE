﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using Infrastructure.IRepositories;
using Domain.Entities;
using Application.DTOs;
using Application.Common.Constants;
using Infrastructure.Repositories;
namespace Infrastructure.Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IClassRepository _classRepository;
        public LessonService(ILessonRepository lessonRepository, IClassRepository classRepository)
        {
            _lessonRepository = lessonRepository;
            _classRepository = classRepository;
        }
        public async Task<OperationResult<bool>> CreateLessonAsync(LessonCreateCommand request)
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

            var success = await _lessonRepository.CreateAsync(newLesson);
            return success
                ? OperationResult<bool>.Ok(true, OperationMessages.CreateSuccess("tiết học"))
                : OperationResult<bool>.Fail(OperationMessages.CreateFail("tiết học"));
        }

        public async Task<OperationResult<bool>> UpdateLessonAsync(LessonUpdateCommand request)
        {
            var lessonFound = await _lessonRepository.GetLessonByClassLessonIDAsync(request.ClassLessonID);
            if (lessonFound == null)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("tiết học"));
            lessonFound.LecturerID = request.LecturerID;
            lessonFound.StartTime = request.StartTime;
            var success = await _lessonRepository.UpdateAsync(lessonFound);
            return success
                ? OperationResult<bool>.Ok(true, OperationMessages.UpdateSuccess("tiết học"))
                : OperationResult<bool>.Fail(OperationMessages.UpdateFail("tiết học"));
        }


        public async Task<OperationResult<List<LessonDTO>>> GetLessonsByClassID(string classID)
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

            return OperationResult<List<LessonDTO>>.Ok(result, OperationMessages.RetrieveSuccess("tiết học"));
        }

        public async Task<OperationResult<List<LessonDTO>>> GetLessonsByStudentID(string studentID)
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

            return OperationResult<List<LessonDTO>>.Ok(result, OperationMessages.RetrieveSuccess("tiết học"));
        }

        public async Task<OperationResult<List<LessonDTO>>> GetLessonsByLecturerID(string lecturerID)
        {
            var lessons = await _lessonRepository.GetLessonsByLecturerIDAsync(lecturerID);
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

            return OperationResult<List<LessonDTO>>.Ok(result, OperationMessages.RetrieveSuccess("tiết học"));
        }

        public async Task<OperationResult<bool>> DeleteLessonAsync(string classLessonID)
        {
            var success = await _lessonRepository.DeleteAsync(classLessonID);
            return success
                ? OperationResult<bool>.Ok(true, OperationMessages.DeleteSuccess("tiết học"))
                : OperationResult<bool>.Fail(OperationMessages.DeleteFail("tiết học"));
        }
        public async Task<OperationResult<bool>> DeleteLessonByClassIDAsync(string classID)
        {
            return await _lessonRepository.DeleteLessonByClassIDAsync(classID);
        }

        public async Task<OperationResult<LessonDetailDTO>> GetLessonDetailByLessonIDAsync(string classLessonID)
        {
            var lesson = await _lessonRepository.GetLessonDetailByLessonIDAsync(classLessonID);
            if (lesson == null)
            {
                return OperationResult<LessonDetailDTO>.Fail(OperationMessages.NotFound("tiết học"));
            }

            return OperationResult<LessonDetailDTO>.Ok(lesson, OperationMessages.RetrieveSuccess("tiết học"));
        }


        public async Task<OperationResult<bool>> CreateLessonsFromSchedulesAsync(
             string classId,
             string lecturerId,
             TimeOnly startHour,
             List<DayOfWeek> selectedDays,
             List<SyllabusScheduleCreateLessonDTO> schedules,
             DateTime startTime
         )
        {
            try
            {
                var lessonsToCreate = new List<Lesson>();
                var startDate = startTime.Date;
                int baseWeek = schedules.Min(s => s.Week);

                var numLesson = await _lessonRepository.CountAsync();

                for (int i = 0; i < schedules.Count; i++)
                {
                    var schedule = schedules[i];
                    DateTime lessonDate;

                    if (i == 0)
                    {
                        lessonDate = startTime.Date;
                    }
                    else
                    {
                        int weekOffset = schedule.Week - baseWeek;
                        lessonDate = GetNextWeekday(startDate, schedule.DayOfWeek, weekOffset);
                    }

                    string lessonID = "L" + (numLesson + lessonsToCreate.Count).ToString("D6");
                    string roomUrl = "https://meet.jit.si/hangullearningsystem/" + Guid.NewGuid().ToString("N").Substring(0, 16);

                    lessonsToCreate.Add(new Lesson
                    {
                        ClassLessonID = lessonID,
                        ClassID = classId,
                        LecturerID = lecturerId,
                        SyllabusScheduleID = schedule.SyllabusScheduleId,
                        StartTime = lessonDate.Add(startHour.ToTimeSpan()),
                        LinkMeetURL = roomUrl,
                        IsActive = true
                    });
                }

                var saveResult = await _lessonRepository.CreateManyAsync(lessonsToCreate);
                if (!saveResult)
                    return OperationResult<bool>.Fail(OperationMessages.CreateFail("buổi học"));

                return OperationResult<bool>.Ok(true, OperationMessages.CreateSuccess("buổi học"));
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail($"Lỗi tạo buổi học: {ex.Message}");
            }
        }

        private DateTime GetNextWeekday(DateTime start, DayOfWeek targetDay, int weekOffset)
        {
            int daysToAdd = ((int)targetDay - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd + 7 * weekOffset);
        }



        public async Task<OperationResult<List<LessonContentDTO>>> GetLessonContentByClassIdAsyn(string classId)
        {
            var classFound = await _classRepository.GetByIdAsync(classId);
            if (!classFound.Success || classFound.Data == null)
                return OperationResult<List<LessonContentDTO>>.Fail(OperationMessages.NotFound("lớp học"));
            return await _lessonRepository.GetLessonContentByClassIdAsyn(classId);
        }

        //kit {Truy vấn danh sách Lesson theo ClassID để phục vụ việc lấy TestEvent của lớp}
        public async Task<OperationResult<List<Lesson>>> GetLessonsByClassIDAsync(string classID)
        {
            try
            {
                var lessons = await _lessonRepository.GetByClassIDAsync(classID);

                // Nếu không có lesson nào thì vẫn trả về danh sách rỗng
                return OperationResult<List<Lesson>>.Ok(lessons);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Lesson>>.Fail("Lỗi khi truy vấn danh sách lesson: " + ex.Message);
            }
        }
    }
}
