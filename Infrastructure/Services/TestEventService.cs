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
        private readonly ITestService _testService;
        private readonly ITestSectionService _testSectionService;
        private readonly IQuestionService _questionService;
        private readonly IMCQOptionService _mcqOptionService;
        public TestEventService(ITestEventRepository testEventRepository, IClassRepository classRepository, ILessonRepository lessonRepository, ISyllabusScheduleTestRepository syllabusScheduleTestRepository, ITestService testService,
        ITestSectionService testSectionService,
        IQuestionService questionService,
        IMCQOptionService mcqOptionService,
        ITestRepository testRepository)
        {
            _testEventRepository = testEventRepository;
            _classRepository = classRepository;
            _lessonRepository = lessonRepository;
            _syllabusScheduleTestRepository = syllabusScheduleTestRepository;
            _testService = testService;
            _testSectionService = testSectionService;
            _questionService = questionService;
            _mcqOptionService = mcqOptionService;
            _testRepository = testRepository;

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

            var testResult = await _testService.GetTestByIdAsync(testEvent.TestID);
            if (!testResult.Success)
                return OperationResult<TestAssignmentDTO>.Fail(testResult.Message);

            var testSectionsResult = await _testSectionService.GetTestSectionsByTestIdAsync(testEvent.TestID);
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

                var questionsResult = await _questionService.GetQuestionsByTestSectionIDAsync(section.TestSectionID);
                if (!questionsResult.Success)
                    continue;

                foreach (var question in questionsResult.Data)
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
                        var optionResult = await _mcqOptionService.GetOptionsByQuestionIDAsync(question.QuestionID);
                        if (optionResult.Success)
                        {
                            questionDTO.Options = optionResult.Data.Select(opt => new MCQOptionAssignmentDTO
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
    }
}
