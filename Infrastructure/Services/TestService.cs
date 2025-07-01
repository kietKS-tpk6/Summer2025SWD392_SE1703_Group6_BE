using Application.IServices;
using Application.Usecases.Command;
using Application.Common.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Repositories;
using Application.DTOs;
using Microsoft.Identity.Client;

namespace Infrastructure.Services
{
    public class TestService : ITestService
    {
        private readonly ILessonService _classLessonService;
        private readonly ITestEventService _testEventService;
        private readonly ITestRepository _testRepository;
        private readonly IAccountService _accountService;
        private readonly ISubjectService _subjectService;
        private readonly ISystemConfigService _systemConfigService;

        private readonly IWritingAnswerRepository _writingAnswerRepository;
        private readonly ITestSectionRepository _testSectionRepository;
        private readonly IStudentTestRepository _studentTestRepository;
        private readonly IQuestionRepository _questionRepo;
        private readonly IMCQOptionRepository _mcqOptionRepository;
        private readonly IMCQAnswerRepository _mcqAnswerRepository;
        private readonly IMCQAnswerDetailRepository _mcqAnswerDetailRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IStudentMarkRepository _studentMarkRepository;
        private readonly ITestEventRepository _testEventRepository;

        


        public TestService(
            ITestRepository testRepository,
            IAccountService accountService,
            ISubjectService subjectService,
            ISystemConfigService systemConfigService,
            IWritingAnswerRepository writingAnswerRepository,
            ITestSectionRepository testSectionRepository,
            IStudentTestRepository studentTestRepository,
            IQuestionRepository questionRepository,
            IStudentMarkRepository studentMarkRepository,
            IAccountRepository accountRepository,
            IMCQAnswerDetailRepository mCQAnswerDetailRepository,
            IMCQOptionRepository mCQOptionRepository,
            IMCQAnswerRepository mCQAnswerRepository,
            ITestEventRepository testEventRepository)
        {
            _testRepository = testRepository;
            _accountService = accountService;
            _subjectService = subjectService;
            _systemConfigService = systemConfigService;
            _writingAnswerRepository = writingAnswerRepository;
            _testSectionRepository = testSectionRepository;
            _studentTestRepository = studentTestRepository;
            _questionRepo = questionRepository;
            _studentMarkRepository = studentMarkRepository;
            _accountRepository = accountRepository;
            _mcqAnswerDetailRepository = mCQAnswerDetailRepository;
            _mcqOptionRepository = mCQOptionRepository;
            _mcqAnswerRepository = mCQAnswerRepository;
            _testEventRepository= testEventRepository;

        }

        public async Task<OperationResult<string>> CreateTestAsync(CreateTestCommand command)
        {
            try
            {
                var accountResult = await _accountService.GetAccountByIdAsync(command.AccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;
                if (!string.Equals(account.Role, "Lecture", StringComparison.OrdinalIgnoreCase) &&
     !string.Equals(account.Role, "Manager", StringComparison.OrdinalIgnoreCase))
                {
                    return OperationResult<string>.Fail("Only lecturers and managers can create tests");
                }

                var subject = await _subjectService.GetSubjectByIdAsync(command.SubjectID);
                if (subject == null)
                    return OperationResult<string>.Fail("Subject not found");

                var testId = await GenerateNextTestIdAsync();
                var test = new Test
                {
                    TestID = testId,
                    CreateBy = command.AccountID,
                    SubjectID = command.SubjectID,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    Status = TestStatus.Drafted,
                    Category = command.Category,
                    TestType = command.TestType,
                    TestName = command.TestName,
                };

                return await _testRepository.CreateTestAsync(test);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error creating test: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> UpdateTestAsync(UpdateTestCommand command)
        {
            try
            {
                var testResult = await _testRepository.GetTestByIdAsync(command.TestID);
                if (!testResult.Success)
                    return OperationResult<string>.Fail(testResult.Message);

                var test = testResult.Data;

                if (test.CreateBy != command.RequestingAccountID)
                    return OperationResult<string>.Fail("You can only update tests you created");

                if (test.Status != TestStatus.Drafted)
                    return OperationResult<string>.Fail("Can only update tests in Draft status");

                return await _testRepository.UpdateTestAsync(test);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error updating test: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> UpdateTestStatusAsync(UpdateTestStatusCommand command)
        {
            try
            {
                var testResult = await _testRepository.GetTestByIdAsync(command.TestID);
                if (!testResult.Success)
                    return OperationResult<string>.Fail(testResult.Message);

                var test = testResult.Data;

                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;

                var canTransition = CanTransitionStatus(
                    test.Status,
                    command.NewStatus,
                    account.Role, 
                    test.CreateBy == command.RequestingAccountID
                );

                if (!canTransition.Success)
                    return OperationResult<string>.Fail(canTransition.Message);

                return await _testRepository.UpdateTestStatusAsync(command.TestID, command.NewStatus);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error updating test status: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> DeleteTestAsync(DeleteTestCommand command)
        {
            try
            {
                var testResult = await _testRepository.GetTestByIdAsync(command.TestID);
                if (!testResult.Success)
                    return OperationResult<string>.Fail(testResult.Message);

                var test = testResult.Data;

                var accountResult = await _accountService.GetAccountByIdAsync(command.RequestingAccountID);
                if (!accountResult.Success)
                    return OperationResult<string>.Fail("Account not found");

                var account = accountResult.Data;
                if (test.CreateBy != command.RequestingAccountID &&
                    !string.Equals(account.Role, "Manager", StringComparison.OrdinalIgnoreCase))
                {
                    return OperationResult<string>.Fail("You don't have permission to delete this test");
                }

                return await _testRepository.DeleteTestAsync(command.TestID);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail($"Error deleting test: {ex.Message}");
            }
        }

        public async Task<OperationResult<Test>> GetTestByIdAsync(string testId)
        {
            return await _testRepository.GetTestByIdAsync(testId);
        }

        public async Task<OperationResult<List<Test>>> GetTestsByAccountIdAsync(string accountId)
        {
            return await _testRepository.GetTestsByAccountIdAsync(accountId);
        }

        public async Task<string> GenerateNextTestIdAsync()
        {
            return await _testRepository.GenerateNextTestIdAsync();
        }

        public async Task ProcessAutoApprovalAsync()
        {
            try
            {
                var configResult = await _systemConfigService.GetConfig("auto_approve_test_after_pending_duration");
                int days = 3; 

                if (configResult.Success && configResult.Data != null && !string.IsNullOrEmpty(configResult.Data.Value))
                {
                    if (!int.TryParse(configResult.Data.Value, out days))
                        days = 3; 
                }

                var pendingTests = await _testRepository.GetPendingTestsOlderThanDaysAsync(days);

                foreach (var test in pendingTests)
                {
                    await _testRepository.UpdateTestStatusAsync(test.TestID, TestStatus.Actived);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in auto approval process: {ex.Message}");
            }
        }

        private OperationResult<string> CanTransitionStatus(TestStatus currentStatus, TestStatus newStatus, string userRole, bool isCreator)
        {
            switch (currentStatus)
            {
                case TestStatus.Drafted:
                    if (newStatus == TestStatus.Pending && isCreator)
                        return OperationResult<string>.Ok("");
                    break;

                case TestStatus.Pending:
                    if (string.Equals(userRole, "Manager", StringComparison.OrdinalIgnoreCase))
                    {
                        if (newStatus == TestStatus.Actived || newStatus == TestStatus.Rejected)
                            return OperationResult<string>.Ok("");
                    }
                    break;

                case TestStatus.Actived:
                    if (string.Equals(userRole, "Manager", StringComparison.OrdinalIgnoreCase) && newStatus == TestStatus.Deleted)
                        return OperationResult<string>.Ok("");
                    break;

                case TestStatus.Rejected:
                    if (isCreator && newStatus == TestStatus.Pending)
                        return OperationResult<string>.Ok("");
                    break;
            }

            return OperationResult<string>.Fail("Invalid status transition");
        }

        public async Task<OperationResult<List<Test>>> GetAllTestsAsync()
        {
            return await _testRepository.GetAllTestsAsync();
        }

        public async Task<OperationResult<List<Test>>> GetAllTestsWithFiltersAsync(string? status = null, string? createdBy = null)
        {
            return await _testRepository.GetAllTestsWithFiltersAsync(status, createdBy);
        }
        public async Task<OperationResult<StudentTestResultDTO>> GetStudentTestResultsByTestEventAsync(string testEventId, string accountId)
        {
            try
            {
                // 1. Lấy danh sách StudentTest theo TestEventID
                var studentTests = await _studentTestRepository.GetByTestEventIdAsync(testEventId);

                if (studentTests == null || !studentTests.Any())
                    return OperationResult<StudentTestResultDTO>.Fail("Không tìm thấy bài kiểm tra nào cho sự kiện này.");

                // 2. Lọc ra bài kiểm tra của học sinh có accountId
                var studentTest = studentTests.FirstOrDefault(st => st.StudentID == accountId);
                if (studentTest == null)
                    return OperationResult<StudentTestResultDTO>.Fail("Không tìm thấy bài làm của học sinh.");

                // 3. Lấy thông tin TestEvent để có TestID
                var testEvent = await _testEventRepository.GetByIdAsync(testEventId);
                if (testEvent == null)
                    return OperationResult<StudentTestResultDTO>.Fail("Không tìm thấy sự kiện kiểm tra.");

                // 4. Lấy thông tin học sinh
                var student = await _accountRepository.GetAccountsByIdAsync(studentTest.StudentID);
                string studentName = student?.FirstName + " " + student?.LastName ?? "Unknown";

                // 5. Lấy điểm chốt từ StudentMarks
                var studentMark = await _studentMarkRepository.GetByStudentTestIdAsync(studentTest.StudentTestID);

                // 6. Lấy các section của bài test
                var sections = await _testSectionRepository.GetByTestIdAsync(testEvent.TestID);
                if (sections == null || !sections.Any())
                    return OperationResult<StudentTestResultDTO>.Fail("Bài kiểm tra không có phần nào.");

                var sectionResults = new List<TestSectionWithStudentAnswersDTO>();

                foreach (var section in sections)
                {
                    var questions = (await _questionRepo.GetQuestionBySectionId(section.TestSectionID))
                                    .Where(q => q.IsActive).ToList();

                    var questionResults = new List<QuestionWithStudentAnswerDTO>();

                    foreach (var question in questions)
                    {
                        var questionDto = new QuestionWithStudentAnswerDTO
                        {
                            QuestionID = question.QuestionID,
                            Context = question.Context,
                            ImageURL = question.ImageURL,
                            AudioURL = question.AudioURL,
                            Type = question.Type ?? TestFormatType.Writing,
                            Score = question.Score,
                            IsActive = question.IsActive,
                            Options = null,
                            StudentAnswer = null
                        };

                        if (section.TestSectionType == TestFormatType.Multiple || section.TestSectionType == TestFormatType.TrueFalse)
                        {
                            var options = await _mcqOptionRepository.GetByQuestionIdAsync(question.QuestionID);
                            var mcqAnswer = await _mcqAnswerRepository.GetByStudentTestAndQuestionAsync(
                                studentTest.StudentTestID, question.QuestionID);

                            List<string> selectedOptionIds = new List<string>();
                            if (mcqAnswer != null)
                            {
                                var answerDetails = await _mcqAnswerDetailRepository.GetByMCQAnswerIdAsync(mcqAnswer.MCQAnswerID);
                                selectedOptionIds = answerDetails.Select(ad => ad.MCQOptionID).ToList();
                            }

                            questionDto.Options = options.Select(o => new MCQOptionWithAnswerDTO
                            {
                                MCQOptionID = o.MCQOptionID,
                                Context = o.Context,
                                ImageURL = o.ImageURL,
                                AudioURL = o.AudioURL,
                                IsCorrect = o.IsCorrect,
                                IsSelected = selectedOptionIds.Contains(o.MCQOptionID)
                            }).ToList();

                            questionDto.StudentAnswer = new StudentAnswerDetailDTO
                            {
                                MCQAnswerID = mcqAnswer?.MCQAnswerID,
                                SelectedOptionIDs = selectedOptionIds
                            };
                        }
                        else if (section.TestSectionType == TestFormatType.Writing)
                        {
                            var writingAnswer = await _writingAnswerRepository.GetByStudentTestAndQuestionAsync(
                                studentTest.StudentTestID, question.QuestionID);

                            questionDto.StudentAnswer = new StudentAnswerDetailDTO
                            {
                                WritingAnswerID = writingAnswer?.WritingAnswerID,
                                StudentEssay = writingAnswer?.StudentEssay,
                                Feedback = writingAnswer?.Feedback,
                                WritingScore = writingAnswer?.Score
                            };
                        }

                        questionResults.Add(questionDto);
                    }

                    sectionResults.Add(new TestSectionWithStudentAnswersDTO
                    {
                        TestSectionID = section.TestSectionID,
                        Context = section.Context,
                        TestSectionType = section.TestSectionType,
                        Score = section.Score,
                        Questions = questionResults
                    });
                }

                var finalResult = new StudentTestResultDTO
                {
                    StudentTestID = studentTest.StudentTestID,
                    StudentID = studentTest.StudentID,
                    StudentName = studentName,
                    TestID = testEvent.TestID,
                    StartTime = studentTest.StartTime,
                    SubmitTime = studentTest.SubmitTime,
                    Status = studentTest.Status.ToString(),
                    InitialMark = studentTest.Mark,
                    Comment = studentMark?.Comment,
                    Sections = sectionResults
                };

                return OperationResult<StudentTestResultDTO>.Ok(finalResult);
            }
            catch (Exception ex)
            {
                return OperationResult<StudentTestResultDTO>.Fail($"Lỗi khi lấy kết quả bài kiểm tra: {ex.Message}");
            }
        }


        public async Task<OperationResult<List<StudentTestResultDTO>>> GetListStudentTestResultsByTestEventAsync(string testEventId)
        {
            try
            {
                // 1. Lấy danh sách StudentTest theo TestEventID
                var studentTests = await _studentTestRepository.GetByTestEventIdAsync(testEventId);

                if (studentTests == null || !studentTests.Any())
                    return OperationResult<List<StudentTestResultDTO>>.Fail("Không tìm thấy bài kiểm tra nào cho sự kiện này.");

                // 2. Lấy thông tin TestEvent để có TestID
                var testEvent = await _testEventRepository.GetByIdAsync(testEventId);
                if (testEvent == null)
                    return OperationResult<List<StudentTestResultDTO>>.Fail("Không tìm thấy sự kiện kiểm tra.");

                var result = new List<StudentTestResultDTO>();

                foreach (var studentTest in studentTests)
                {
                    // 3. Lấy thông tin học sinh
                    var student = await _accountRepository.GetAccountsByIdAsync(studentTest.StudentID);
                    string studentName = student?.FirstName + " " + student?.LastName ?? "Unknown";

                    // 4. Lấy điểm chốt từ StudentMarks
                    var studentMark = await _studentMarkRepository.GetByStudentTestIdAsync(studentTest.StudentTestID);

                    // 5. Lấy thông tin Test và các Section - sử dụng TestID từ TestEvent
                    var sections = await _testSectionRepository.GetByTestIdAsync(testEvent.TestID);
                    if (sections == null || !sections.Any()) continue;

                    var sectionResults = new List<TestSectionWithStudentAnswersDTO>();

                    foreach (var section in sections)
                    {
                        // 6. Lấy danh sách câu hỏi trong section
                        var questions = (await _questionRepo.GetQuestionBySectionId(section.TestSectionID))
                                       .Where(q => q.IsActive).ToList();

                        var questionResults = new List<QuestionWithStudentAnswerDTO>();

                        foreach (var question in questions)
                        {
                            var questionDto = new QuestionWithStudentAnswerDTO
                            {
                                QuestionID = question.QuestionID,
                                Context = question.Context,
                                ImageURL = question.ImageURL,
                                AudioURL = question.AudioURL,
                                Type = question.Type ?? TestFormatType.Writing,
                                Score = question.Score,
                                IsActive = question.IsActive,
                                Options = null,
                                StudentAnswer = null
                            };

                            // 7. Xử lý theo loại câu hỏi
                            if (section.TestSectionType == TestFormatType.Multiple || section.TestSectionType == TestFormatType.TrueFalse)
                            {
                                // Lấy các option của câu hỏi
                                var options = await _mcqOptionRepository.GetByQuestionIdAsync(question.QuestionID);

                                // Lấy đáp án học sinh cho câu hỏi MCQ
                                var mcqAnswer = await _mcqAnswerRepository.GetByStudentTestAndQuestionAsync(
                                    studentTest.StudentTestID, question.QuestionID);

                                List<string> selectedOptionIds = new List<string>();
                                if (mcqAnswer != null)
                                {
                                    var answerDetails = await _mcqAnswerDetailRepository.GetByMCQAnswerIdAsync(mcqAnswer.MCQAnswerID);
                                    selectedOptionIds = answerDetails.Select(ad => ad.MCQOptionID).ToList();
                                }

                                questionDto.Options = options.Select(o => new MCQOptionWithAnswerDTO
                                {
                                    MCQOptionID = o.MCQOptionID,
                                    Context = o.Context,
                                    ImageURL = o.ImageURL,
                                    AudioURL = o.AudioURL,
                                    IsCorrect = o.IsCorrect,
                                    IsSelected = selectedOptionIds.Contains(o.MCQOptionID)
                                }).ToList();

                                questionDto.StudentAnswer = new StudentAnswerDetailDTO
                                {
                                    MCQAnswerID = mcqAnswer?.MCQAnswerID,
                                    SelectedOptionIDs = selectedOptionIds
                                };
                            }
                            else if (section.TestSectionType == TestFormatType.Writing)
                            {
                                // Lấy đáp án học sinh cho câu hỏi Writing
                                var writingAnswer = await _writingAnswerRepository.GetByStudentTestAndQuestionAsync(
                                    studentTest.StudentTestID, question.QuestionID);

                                questionDto.StudentAnswer = new StudentAnswerDetailDTO
                                {
                                    WritingAnswerID = writingAnswer?.WritingAnswerID,
                                    StudentEssay = writingAnswer?.StudentEssay,
                                    Feedback = writingAnswer?.Feedback,
                                    WritingScore = writingAnswer?.Score
                                };
                            }

                            questionResults.Add(questionDto);
                        }

                        sectionResults.Add(new TestSectionWithStudentAnswersDTO
                        {
                            TestSectionID = section.TestSectionID,
                            Context = section.Context,
                            TestSectionType = section.TestSectionType,
                            Score = section.Score,
                            Questions = questionResults
                        });
                    }

                    result.Add(new StudentTestResultDTO
                    {
                        StudentTestID = studentTest.StudentTestID,
                        StudentID = studentTest.StudentID,
                        StudentName = studentName,
                        TestID = testEvent.TestID,  // Lấy TestID từ TestEvent
                        StartTime = studentTest.StartTime,
                        SubmitTime = studentTest.SubmitTime,
                        Status = studentTest.Status.ToString(),
                        InitialMark = studentTest.Mark,  // Điểm ban đầu
                        Comment = studentMark?.Comment,  // Nhận xét
                        Sections = sectionResults
                    });
                }

                return OperationResult<List<StudentTestResultDTO>>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<List<StudentTestResultDTO>>.Fail($"Lỗi khi lấy kết quả bài kiểm tra: {ex.Message}");
            }
        }

        //Kho - làm tạm hàm update status mới
        public async Task<OperationResult<string>> UpdateTestStatusFixAsync(UpdateTestStatusFixCommand request)
        {
            var testFound = await _testRepository.GetByIdAsync(request.TestID);
            if (testFound == null)
            {
                return OperationResult<string>.Fail(OperationMessages.NotFound("đề kiểm tra"));
            }
            testFound.Status = request.TestStatus; 
            return await _testRepository.UpdateTestAsync(testFound);
        }
    }
}