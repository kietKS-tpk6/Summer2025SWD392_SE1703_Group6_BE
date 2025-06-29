using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class TestEventService : ITestEventService
    {
        private readonly ITestEventRepository _testEventRepository;
        private readonly IClassRepository _classRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ISyllabusScheduleTestRepository _syllabusScheduleTestRepository;
        private readonly ITestRepository _testRepository;

        private readonly ITestSectionRepository _testSectionRepository;
        private readonly IQuestionRepository _questionRepo;
        private readonly IMCQOptionRepository _mCQOptionRepository;

        public TestEventService(ITestEventRepository testEventRepository, IClassRepository classRepository,
            ILessonRepository lessonRepository,
            ISyllabusScheduleTestRepository syllabusScheduleTestRepository,
        ITestSectionRepository testSectionRepository,
        IQuestionRepository questionRepository,
        ITestRepository testRepository,
        IMCQOptionRepository mCQOptionRepository)
        {
            _testEventRepository = testEventRepository;
            _classRepository = classRepository;
            _lessonRepository = lessonRepository;
            _syllabusScheduleTestRepository = syllabusScheduleTestRepository;
            _testSectionRepository = testSectionRepository;
            _questionRepo = questionRepository;
            _testRepository = testRepository;
            _mCQOptionRepository = mCQOptionRepository;
        }
        public async Task<OperationResult<bool>> SetupTestEventsByClassIDAsync(string classID)
        {
            var classes = await _classRepository.GetByIdAsync(classID);
            if (classes == null)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("lớp học"));

            var lessons = await _lessonRepository.GetLessonsByClassIDAsync(classID);
            if (lessons == null || !lessons.Any())
                return OperationResult<bool>.Fail(OperationMessages.NotFound("tiết học trong lớp"));

            int createdCount = 0;

            foreach (var lesson in lessons)
            {
                if (lesson?.SyllabusSchedule == null || !lesson.SyllabusSchedule.HasTest)
                    continue;

                var scheduleTest = await _syllabusScheduleTestRepository
                    .GetSyllabusScheduleTestBySyllabusScheduleIdAsync(lesson.SyllabusScheduleID);

                if (scheduleTest == null)
                    continue;

                var countResult = await _testEventRepository.CountTestEventAsync();
                if (!countResult.Success)
                    return OperationResult<bool>.Fail(countResult.Message);
                var newTestEventId = "TE" + countResult.Data.ToString("D4"); 
                var randomPassword = Guid.NewGuid().ToString("N")[..10];

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
                    ScheduleTestID = scheduleTest.ScheduleTestID,
                    AttemptLimit = 1,
                    Password = randomPassword,
                    ClassLessonID = lesson.ClassLessonID
                };

                await _testEventRepository.CreateTestEventForCreateClassAsync(newTestEvent);
                createdCount++;
            }

            if (createdCount == 0)
                return OperationResult<bool>.Fail("Không có buổi học nào có kiểm tra để tạo sự kiện.");

            return OperationResult<bool>.Ok(true, $"Đã tạo {createdCount} sự kiện kiểm tra cho lớp.");
        }

        public async Task<OperationResult<bool>> DeleteTestEventsByClassIDAsync(string classID)
        {
            return await _testEventRepository.DeleteTestEventsByClassIDAsync(classID);
        }

        public async Task<OperationResult<TestAssignmentDTO>> GetTestAssignmentForStudentAsync(string testEventID)
        {
            var testEvent = await _testEventRepository.GetByIdAsync(testEventID);
            if (testEvent == null || testEvent.Status == TestEventStatus.Deleted)
                return OperationResult<TestAssignmentDTO>.Fail("Test event not found or deleted");

            if (string.IsNullOrEmpty(testEvent.TestID))
                return OperationResult<TestAssignmentDTO>.Fail("Test not assigned to this event yet");

            var testResult = await _testRepository.GetTestByIdAsync(testEvent.TestID);
            if (!testResult.Success)
                return OperationResult<TestAssignmentDTO>.Fail(testResult.Message);

            var testSectionsResult = await _testSectionRepository.GetTestSectionsByTestIdAsync(testEvent.TestID);
            if (!testSectionsResult.Success)
                return OperationResult<TestAssignmentDTO>.Fail(testSectionsResult.Message);

            var testAssignment = new TestAssignmentDTO
            {
                TestID = testEvent.TestID,
                Sections = new List<TestSectionAssignmentDTO>()
            };

            foreach (var section in testSectionsResult.Data)
            {
                var sectionDTO = new TestSectionAssignmentDTO
                {
                    TestSectionID = section.TestSectionID,
                    Context = section.Context,
                    ImageURL = section.ImageURL,
                    AudioURL = section.AudioURL,
                    FormatType = section.TestSectionType,
                    Score = section.Score,
                    Questions = new List<QuestionAssignmentDTO>()
                };

                var questions = await _questionRepo.GetQuestionBySectionId(section.TestSectionID);
                var activeQuestions = questions.Where(q => q.IsActive).ToList();

                foreach (var question in activeQuestions)

                { 
                    var questionDTO = new QuestionAssignmentDTO
                    {
                        QuestionID = question.QuestionID,
                        Content = question.Context,
                        ImageURL = question.ImageURL,
                        AudioURL = question.AudioURL,
                    };

                    if (section.TestSectionType == TestFormatType.Multiple || section.TestSectionType == TestFormatType.TrueFalse)
                    {
                        var options = await _mCQOptionRepository.GetByQuestionIdAsync(question.QuestionID);

                        if (options != null && options.Any())
                        {
                            questionDTO.Options = options.Select(opt => new MCQOptionAssignmentDTO
                            {
                                OptionID = opt.MCQOptionID,
                                Context = opt.Context,
                                ImageURL = opt.ImageURL,
                                AudioURL = opt.AudioURL
                            }).ToList();
                        }

                    }

                    sectionDTO.Questions.Add(questionDTO);
                }

                testAssignment.Sections.Add(sectionDTO);
            }

            return OperationResult<TestAssignmentDTO>.Ok(testAssignment);
        }
        //kit {Lấy danh sách tất cả TestEvent theo ClassID, thông qua ClassLessonID}
        public async Task<OperationResult<List<TestByClassDTO>>> GetTestsByClassIDAsync(string classID)
        {
            List<Lesson> lessons;
            try
            {
                lessons = await _lessonRepository.GetByClassIDAsync(classID);
            }
            catch (Exception ex)
            {
                return OperationResult<List<TestByClassDTO>>.Fail("Lỗi khi truy vấn danh sách lesson: " + ex.Message);
            }

            var lessonIDs = lessons.Select(l => l.ClassLessonID).ToList();
            var testEvents = await _testEventRepository.GetByClassLessonIDsAsync(lessonIDs);
            var result = new List<TestByClassDTO>();
            foreach (var ev in testEvents)
            {
                var test = await _testRepository.GetByIdAsync(ev.TestID);

                result.Add(new TestByClassDTO
                {
                    TestEventID = ev.TestEventID,
                    TestID = ev.TestID,
                    TestCategory = test?.Category.ToString(),
                    TestName = test?.TestName,
                    Description = ev.Description,
                    StartAt = ev.StartAt,
                    EndAt = ev.EndAt,
                    TestType = ev.TestType.ToString(),
                    Status = ev.Status.ToString()
                });
            }

            return OperationResult<List<TestByClassDTO>>.Ok(result);
        }
    
    }
}
