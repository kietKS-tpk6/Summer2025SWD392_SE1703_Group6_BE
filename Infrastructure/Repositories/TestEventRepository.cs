using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TestEventRepository : ITestEventRepository
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

        public async Task<OperationResult<List<TestEventWithLessonDTO>>> GetTestEventWithLessonsByClassIDAsync(string classID)
        {
            var result = await (from te in _dbContext.TestEvent
                                join lesson in _dbContext.Lesson
                                    on te.ClassLessonID equals lesson.ClassLessonID
                                join ss in _dbContext.SyllabusSchedule
                                    on lesson.SyllabusScheduleID equals ss.SyllabusScheduleID
                                join sst in _dbContext.SyllabusScheduleTests
                                    on te.ScheduleTestID equals sst.ScheduleTestID into sstJoin
                                from sst in sstJoin.DefaultIfEmpty()
                                join ac in _dbContext.AssessmentCriteria
                                    on sst.AssessmentCriteriaID equals ac.AssessmentCriteriaID into acJoin
                                from ac in acJoin.DefaultIfEmpty()
                                where lesson.ClassID == classID && te.Status != TestEventStatus.Deleted
                                orderby te.TestID == null ? 0 : 1, te.StartAt
                                select new TestEventWithLessonDTO
                                {
                                    TestEventID = te.TestEventID,
                                    TestID = te.TestID,
                                    Description = te.Description,
                                    StartAt = te.StartAt,
                                    EndAt = te.EndAt,
                                    DurationMinutes = te.DurationMinutes,
                                    TestType = te.TestType,
                                    Status = te.Status,
                                    ScheduleTestID = te.ScheduleTestID,
                                    AttemptLimit = te.AttemptLimit,
                                    Password = te.Password,
                                    ClassLessonID = te.ClassLessonID,
                                    LessonTitle = ss.LessonTitle,
                                    LessonStartTime = lesson.StartTime,
                                    LessonEndTime = lesson.StartTime.AddMinutes(ss.DurationMinutes ?? 45),
                                    AssessmentCategory = ac.Category
                                })
                                .ToListAsync();

            return result is { Count: > 0 }
                ? OperationResult<List<TestEventWithLessonDTO>>.Ok(result, OperationMessages.RetrieveSuccess("buổi kiểm tra"))
                : OperationResult<List<TestEventWithLessonDTO>>.Fail(OperationMessages.RetrieveFail("buổi kiểm tra"));
        }
        public async Task<OperationResult<List<TestEventStudentDTO>>> GetTestEventByStudentIdAsync(string studentId)
        {
            var classQuery = from ce in _dbContext.ClassEnrollment
                             join c in _dbContext.Class on ce.ClassID equals c.ClassID
                             join s in _dbContext.Subject on c.SubjectID equals s.SubjectID
                             where ce.StudentID == studentId && c.Status == ClassStatus.Ongoing
                             select new
                             {
                                 c.ClassID,
                                 c.ClassName,
                                 SubjectName = s.SubjectName
                             };

            var classInfos = await classQuery.ToListAsync();

            if (!classInfos.Any())
                return OperationResult<List<TestEventStudentDTO>>.Fail("Không tìm thấy lớp đang học của học viên.");

            var classIds = classInfos.Select(c => c.ClassID).ToList();

            var testEventsQuery = from te in _dbContext.TestEvent
                                  join l in _dbContext.Lesson on te.ClassLessonID equals l.ClassLessonID
                                  join ss in _dbContext.SyllabusSchedule on l.SyllabusScheduleID equals ss.SyllabusScheduleID
                                  where classIds.Contains(l.ClassID) && te.Status == TestEventStatus.Actived
                                  select new
                                  {
                                      ClassID = l.ClassID,
                                      TestEvent = new TestEventInClassDTO
                                      {
                                          TestEventID = te.TestEventID,
                                          TestID = te.TestID,
                                          Description = te.Description,
                                          StartAt = te.StartAt,
                                          EndAt = te.EndAt,
                                          DurationMinutes = te.DurationMinutes,
                                          TestType = te.TestType.ToString(),
                                          Status = te.Status,
                                          AttemptLimit = te.AttemptLimit,
                                          Password = te.Password,
                                          LessonTitle = ss.LessonTitle
                                      }
                                  };

            var testEventMap = await testEventsQuery
                .GroupBy(x => x.ClassID)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.TestEvent).ToList());

            var result = classInfos.Select(ci => new TestEventStudentDTO
            {
                ClassID = ci.ClassID,
                ClassName = ci.ClassName,
                SubjectName = ci.SubjectName,
                TestEvents = testEventMap.ContainsKey(ci.ClassID) ? testEventMap[ci.ClassID] : new List<TestEventInClassDTO>()
            }).ToList();

            return OperationResult<List<TestEventStudentDTO>>.Ok(result, OperationMessages.RetrieveSuccess("danh sách buổi kiểm tra"));
        }
        public async Task<OperationResult<TestEventWithLessonDTO>> GetTestEventWithLessonDTOByIDAsync(string testEventID)
        {
            var result = await (
                from te in _dbContext.TestEvent
                join lesson in _dbContext.Lesson on te.ClassLessonID equals lesson.ClassLessonID
                join ss in _dbContext.SyllabusSchedule on lesson.SyllabusScheduleID equals ss.SyllabusScheduleID
                join sst in _dbContext.SyllabusScheduleTests on te.ScheduleTestID equals sst.ScheduleTestID into sstJoin
                from sst in sstJoin.DefaultIfEmpty()
                join ac in _dbContext.AssessmentCriteria on sst.AssessmentCriteriaID equals ac.AssessmentCriteriaID into acJoin
                from ac in acJoin.DefaultIfEmpty()
                where te.TestEventID == testEventID
                select new TestEventWithLessonDTO
                {
                    TestEventID = te.TestEventID,
                    TestID = te.TestID,
                    Description = te.Description,
                    StartAt = te.StartAt,
                    EndAt = te.EndAt,
                    DurationMinutes = te.DurationMinutes,
                    TestType = te.TestType,
                    Status = te.Status,
                    ScheduleTestID = te.ScheduleTestID,
                    AttemptLimit = te.AttemptLimit,
                    Password = te.Password,
                    ClassLessonID = te.ClassLessonID,
                    LessonTitle = ss.LessonTitle,
                    LessonStartTime = lesson.StartTime,
                    LessonEndTime = lesson.StartTime.AddMinutes((double)(ss.DurationMinutes ?? 45)),
                    AssessmentCategory = ac.Category
                }
            ).FirstOrDefaultAsync();

            if (result is null)
            {
                return OperationResult<TestEventWithLessonDTO>.Fail(
                    OperationMessages.RetrieveFail("buổi kiểm tra"));
            }

            return OperationResult<TestEventWithLessonDTO>.Ok(
                result,
                OperationMessages.RetrieveSuccess("buổi kiểm tra"));
        }



        //kit {Lấy tất cả TestEvent theo danh sách ClassLessonID}
        public async Task<List<TestEvent>> GetByClassLessonIDsAsync(List<string> classLessonIDs)
        {
            return await _dbContext.TestEvent
                .Where(te => classLessonIDs.Contains(te.ClassLessonID))
                .ToListAsync();
        }
        //kit {Lấy testID  theo testEvent}

        public async Task<string?> GetTestIDByTestEventIDAsync(string testEventID)
        {
            return await _dbContext.TestEvent
                .Where(te => te.TestEventID == testEventID && te.Status != TestEventStatus.Deleted)
                .Select(te => te.TestID)
                .FirstOrDefaultAsync();
        }
    }
}
