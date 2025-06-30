using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using CloudinaryDotNet.Core;
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
        private readonly ITestRepository _testRepository;
        private readonly ISyllabusScheduleTestRepository _syllabusScheduleTestRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITestSectionRepository _testSectionRepository;
        private readonly IQuestionRepository _questionRepo;
        private readonly IMCQOptionRepository _mCQOptionRepository;
        private readonly IStudentTestRepository _studentTestRepository;

        public TestEventService(ITestEventRepository testEventRepository, IClassRepository classRepository,
            ILessonRepository lessonRepository,
            ISyllabusScheduleTestRepository syllabusScheduleTestRepository,
        ITestSectionRepository testSectionRepository,
        IQuestionRepository questionRepository,
        ITestRepository testRepository,
        IMCQOptionRepository mCQOptionRepository,
        IStudentTestRepository studentTestRepository,
         IAccountRepository accountRepository
        )

        {
            _testEventRepository = testEventRepository;
            _classRepository = classRepository;
            _lessonRepository = lessonRepository;
            _syllabusScheduleTestRepository = syllabusScheduleTestRepository;
            _testSectionRepository = testSectionRepository;
            _questionRepo = questionRepository;
            _testRepository = testRepository;
            _mCQOptionRepository = mCQOptionRepository;
            _studentTestRepository = studentTestRepository;
            _accountRepository = accountRepository;
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

                var newTestEvent = new TestEvent
                {
                    TestEventID = newTestEventId,
                    TestID = null,
                    Description = null,
                    StartAt = lesson.StartTime,
                    EndAt = lesson.StartTime.AddMinutes(lesson.SyllabusSchedule.DurationMinutes ?? 60),
                    DurationMinutes = lesson.SyllabusSchedule.DurationMinutes ?? 60,
                    TestType = scheduleTest.TestType,
                    Status = TestEventStatus.Draft,
                    ScheduleTestID = scheduleTest.ScheduleTestID,
                    AttemptLimit = null,
                    Password = null,
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
            var allStudentTests = await _studentTestRepository.GetByTestEventIDsAsync(testEvents.Select(te => te.TestEventID).ToList());

            var result = new List<TestByClassDTO>();

            foreach (var ev in testEvents)
            {
                var test = await _testRepository.GetByIdAsync(ev.TestID);

                var studentTests = allStudentTests.Where(st => st.TestEventID == ev.TestEventID).ToList();

                int totalSubmitted = studentTests.Count(st => st.SubmitTime != null);
                int uniqueStudents = studentTests
                    .Where(st => st.SubmitTime != null)
                    .Select(st => st.StudentID)
                    .Distinct()
                    .Count();

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
                    Status = ev.Status.ToString(),
                    DurationMinutes = ev.DurationMinutes,
                    AttemptLimit = ev.AttemptLimit.GetValueOrDefault(),
                    TotalSubmittedTests = totalSubmitted,
                    TotalStudentsSubmitted = uniqueStudents
                });
            }

            return OperationResult<List<TestByClassDTO>>.Ok(result);
        }



        public async Task<OperationResult<bool>> UpdateTestEventAsync(UpdateTestEventCommand request)
        {
            var testEventFound = await _testEventRepository.GetByIdAsync(request.TestEventIdToUpdate);
            if (testEventFound == null)
            {
                return OperationResult<bool>.Fail(OperationMessages.UpdateFail("buổi kiểm tra"));
            }

            var durationRequest = (request.EndAt - request.StartAt).TotalMinutes;
            if (durationRequest < testEventFound.DurationMinutes)
            {
                return OperationResult<bool>.Fail("Thời gian kiểm tra không hợp lý.");
            }
            var testFound = await _testRepository.GetTestByIdAsync(request.TestID);
            if (!testFound.Success)
            {
                return OperationResult<bool>.Fail(OperationMessages.NotFound("đề kiểm tra"));
            }
            if (testFound.Data.Status != TestStatus.Actived)
            {
                return OperationResult<bool>.Fail("Không thể chọn đề kiểm tra chưa duyệt");
            }
            var scheduleTestFound = await _syllabusScheduleTestRepository.GetByScheduleTestIdAsync(testEventFound.ScheduleTestID);
            if (scheduleTestFound != null)
            {
                if (scheduleTestFound.TestType != testFound.Data.TestType)
                {
                    return OperationResult<bool>.Fail("Không thể chọn đề kiểm tra khác loại buổi kiểm tra");
                }
            }
            testEventFound.TestID = request.TestID;
            testEventFound.Description = request.Description;
            testEventFound.StartAt = request.StartAt;
            testEventFound.EndAt = request.EndAt;
            testEventFound.AttemptLimit = request.AttemptLimit == 0 ? null : request.AttemptLimit;
            testEventFound.Password = string.IsNullOrWhiteSpace(request.Password) ? null : request.Password;

            return await _testEventRepository.UpdateTestEventAsync(testEventFound);
        }

        public async Task<OperationResult<bool>> UpdateStatusAsync(UpdateStatusTestEventCommand request)
        {
            var testEventFound = await _testEventRepository.GetByIdAsync(request.TestEventIDToUpdate);
            if (testEventFound == null)
            {
                return OperationResult<bool>.Fail(OperationMessages.NotFound("buổi kiểm tra"));
            }
            testEventFound.Status = request.Status;
            return await _testEventRepository.UpdateTestEventAsync(testEventFound);
        }
        public async Task<OperationResult<List<TestEventWithLessonDTO>>> GetTestEventWithLessonsByClassIDAsync(string classID) 
        {
            var classFound = await _classRepository.GetByIdAsync(classID);
            if (!classFound.Success)
            {
                return OperationResult<List<TestEventWithLessonDTO>>.Fail(OperationMessages.NotFound("lớp học"));
            }
            return await _testEventRepository.GetTestEventWithLessonsByClassIDAsync(classFound.Data.ClassID);
            }
        public async Task<OperationResult<List<TestByClassDTO>>> GetMidtermAndFinalTestsByClassIDAsync(string classID)
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

                if (test == null)
                    continue;

                // Chỉ lấy Midterm hoặc Final
                if (test.Category.ToString() == TestCategory.Midterm.ToString() ||
    test.Category.ToString() == TestCategory.Final.ToString())
                {
                    result.Add(new TestByClassDTO
                    {
                        TestEventID = ev.TestEventID,
                        TestID = ev.TestID,
                        TestCategory = test.Category.ToString(),
                        TestName = test.TestName,
                        Description = ev.Description,
                        StartAt = ev.StartAt,
                        EndAt = ev.EndAt,
                        TestType = ev.TestType.ToString(),
                        Status = ev.Status.ToString()
                    });
                }
            }

            return OperationResult<List<TestByClassDTO>>.Ok(result);
        }

      
        public async Task<OperationResult<List<TestEventStudentDTO>>> GetTestEventByStudentIdAsync(string studentId)
        {
            var studentFound = await _accountRepository.GetAccountsByIdAsync(studentId);
            if(studentFound == null)
            {
                return OperationResult<List<TestEventStudentDTO>>.Fail(OperationMessages.NotFound("học sinh"));
            }
            //if(studentFound.Role != AccountRole.Student)
            //{
            //    return OperationResult<TestEventStudentDTO>.Fail("Không phải học sinh");
            //}
            return await  _testEventRepository.GetTestEventByStudentIdAsync(studentId);
        }
    }
}
