using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class TestEventService : ITestEventService
    {
        private readonly ITestEventRepository _testEventRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ISyllabusScheduleTestRepository _syllabusScheduleTestRepository;
        public TestEventService(ITestEventRepository testEventRepository)
        {
            _testEventRepository = testEventRepository;

        }
        public async Task<OperationResult<bool>> CreateTestEventForCreateClassAsync(string lessonID)
        {
            var lesson = await _lessonRepository.GetLessonByClassLessonIDAsync(lessonID);

            if (lesson == null || lesson.SyllabusSchedule == null || !lesson.SyllabusSchedule.HasTest)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("lesson hoặc lesson không có kiểm tra"));

            var scheduleTest = await _syllabusScheduleTestRepository.GetSyllabusScheduleTestByIdAsync(lesson.SyllabusScheduleID);

            if (scheduleTest == null)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("lịch kiểm tra cho tiết học"));

            string randomPassword = Guid.NewGuid().ToString("N").Substring(0, 10);
            var count = await _testEventRepository.CountTestEventAsync();
            var newTestEventId = "TE" + Convert.ToInt32(count).ToString("D4");

            var newTestEvent = new TestEvent
            {
                TestEventID = newTestEventId,
                TestID = null,
                Description = null,
                StartAt = null,
                EndAt = null,
                DurationMinutes = lesson.SyllabusSchedule.DurationMinutes ?? 60,
                TestType = scheduleTest.TestType,
                Status = TestEventStatus.Draft,
                ScheduleTestID = scheduleTest.SyllabusSchedulesID,
                AttemptLimit = 1,
                Password = randomPassword,
                ClassLessonID = lessonID
            };

            await _testEventRepository.CreateTestEventForCreateClassAsync(newTestEvent);

            return OperationResult<bool>.Ok(true, OperationMessages.CreateSuccess("buổi kiểm tra"));
        }



    }
}
