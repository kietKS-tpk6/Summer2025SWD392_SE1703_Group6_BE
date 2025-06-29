using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TestEventRepository: ITestEventRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public TestEventRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OperationResult<bool>> CreateTestEventForCreateClassAsync(TestEvent testEvent)
        {
            if (testEvent == null)
                return OperationResult<bool>.Fail("buổi kiểm tra không hợp lệ");

            _dbContext.TestEvent.Add(testEvent);
            await _dbContext.SaveChangesAsync();

            return OperationResult<bool>.Ok(true, OperationMessages.CreateSuccess("buổi kiểm tra"));
        }

        public async Task<OperationResult<int>> CountTestEventAsync()
        {
            var count = await _dbContext.TestEvent.CountAsync();
            return OperationResult<int>.Ok(count);
        }
        public async Task<OperationResult<bool>> DeleteTestEventsByClassIDAsync(string classId)
        {
            var lessonIdsWithTest = await (
                from lesson in _dbContext.Lesson
                join schedule in _dbContext.SyllabusSchedule
                    on lesson.SyllabusScheduleID equals schedule.SyllabusScheduleID
                where lesson.ClassID == classId && schedule.HasTest
                select lesson.ClassLessonID
            ).ToListAsync();

            var testEvents = await _dbContext.TestEvent
                .Where(te => lessonIdsWithTest.Contains(te.ClassLessonID))
                .ToListAsync();

            foreach (var testEvent in testEvents)
            {
                testEvent.Status = TestEventStatus.Deleted;
                _dbContext.TestEvent.Update(testEvent);
            }

            var result = await _dbContext.SaveChangesAsync();

            return result > 0
                ? OperationResult<bool>.Ok(true, OperationMessages.DeleteSuccess("buổi kiểm tra"))
                : OperationResult<bool>.Fail(OperationMessages.DeleteFail("buổi kiểm tra"));
        }
        public async Task<OperationResult<bool>> UpdateTestEventAsync(TestEvent testEvent)
        {
            _dbContext.TestEvent.Update(testEvent);
            await _dbContext.SaveChangesAsync();
            return OperationResult<bool>.Ok(true, OperationMessages.UpdateSuccess("buổi kiểm tra"));
        }

        public async Task<TestEvent?> GetByIdAsync(string testEventID)
        {
            return await _dbContext.TestEvent
                .FirstOrDefaultAsync(te => te.TestEventID == testEventID && te.Status != TestEventStatus.Deleted);
        }
    }
}
