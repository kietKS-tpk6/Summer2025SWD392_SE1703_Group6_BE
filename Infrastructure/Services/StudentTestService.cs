using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Infrastructure.IRepositories;
using Infrastructure.Data;
using Application.Usecases.Command;
using Infrastructure.Repositories;

namespace Infrastructure.Services
{
    public class StudentTestService : IStudentTestService
    {
        private readonly IStudentTestRepository _studentTestRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly ITestSectionRepository _sectionRepo;
        private readonly IMCQOptionRepository _optionRepo;
        private readonly IMCQAnswerRepository _mcqAnswerRepo;
        private readonly IWritingAnswerRepository _writingAnswerRepo;
        private readonly ITestRepository _testRepo;
        private readonly ITestEventRepository _testEventRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;




        private readonly HangulLearningSystemDbContext _dbContext;

        public StudentTestService(
            IStudentTestRepository studentTestRepo,
            IQuestionRepository questionRepo,
            ITestSectionRepository sectionRepo,
            IMCQOptionRepository optionRepo,
            IMCQAnswerRepository mcqAnswerRepo,
            IWritingAnswerRepository writingAnswerRepo,
            HangulLearningSystemDbContext dbContext,
            ITestRepository testRepository,
            ITestEventRepository testEventRepository,
            ILessonRepository lessonRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _studentTestRepo = studentTestRepo;
            _questionRepo = questionRepo;
            _sectionRepo = sectionRepo;
            _optionRepo = optionRepo;
            _mcqAnswerRepo = mcqAnswerRepo;
            _writingAnswerRepo = writingAnswerRepo;
            _dbContext = dbContext;
            _testRepo = testRepository;
            _testEventRepository = testEventRepository;
            _lessonRepository = lessonRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<OperationResult<bool>> ValidateStudentTestExistsAsync(string studentTestID)
        {
            var exists = await _studentTestRepo.ExistsAsync(studentTestID);
            return exists ? OperationResult<bool>.Ok(true) : OperationResult<bool>.Fail("StudentTest không tồn tại.");
        }
        public async Task<OperationResult<bool>> SubmitStudentTestAsync(
        string studentID,
        string testEventID,
        List<SectionAnswerDTO> sectionAnswers)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                decimal totalScore = 0m;
                bool hasWriting = false;
                bool hasMCQ = false;

                // Generate new StudentTest ID
                string studentTestID = await GenerateStudentTestIDAsync();

                // Create new StudentTest entity
                var studentTest = new StudentTest
                {
                    StudentTestID = studentTestID,
                    StudentID = studentID,
                    TestEventID = testEventID,
                    StartTime = DateTime.Now,
                    SubmitTime = DateTime.Now,
                    Status = StudentTestStatus.Submitted // Will be updated based on test type
                };
                await _studentTestRepo.AddAsync(studentTest);
                await _dbContext.SaveChangesAsync();

                foreach (var section in sectionAnswers)
                {
                    if (section.FormatType == TestFormatType.Writing)
                        hasWriting = true;
                    else
                        hasMCQ = true;

                    foreach (var answer in section.Answers)
                    {
                        if (section.FormatType == TestFormatType.Writing)
                        {
                            var essay = answer.Answers.FirstOrDefault() ?? string.Empty;
                            var saveResult = await _writingAnswerRepo.SaveAnswerAsync(studentTestID, answer.QuestionID, essay);
                            if (!saveResult.Success)
                            {
                                await transaction.RollbackAsync();
                                return OperationResult<bool>.Fail($"Lỗi lưu bài viết: {saveResult.Message}");
                            }
                        }
                        else // MCQ
                        {
                            var saveResult = await _mcqAnswerRepo.SaveAnswerAsync(studentTestID, answer.QuestionID, answer.Answers);
                            if (!saveResult.Success)
                            {
                                await transaction.RollbackAsync();
                                return OperationResult<bool>.Fail($"Lỗi lưu đáp án MCQ: {saveResult.Message}");
                            }

                            var correctIDs = await _optionRepo.GetCorrectOptionIDsAsync(answer.QuestionID);
                            var isCorrect = correctIDs.OrderBy(x => x).SequenceEqual(answer.Answers.OrderBy(x => x));
                            if (isCorrect)
                            {
                                var question = await _questionRepo.GetByIdAsync(answer.QuestionID);
                                if (question != null)
                                    totalScore += question.Score;
                            }
                        }
                    }
                }
                
                    var testID = await _testEventRepository.GetTestIDByTestEventIDAsync(testEventID);
                var test = await _testRepo.GetTestByIdAsync(testID);
                if (test == null)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<bool>.Fail("Không tìm thấy bài kiểm tra.");
                }

                // Determine status based on test type
                var newStatus = test.Data.TestType switch
                {
                    TestType.MCQ => StudentTestStatus.Graded,
                    TestType.Writing => StudentTestStatus.WaitingForWritingGrading,
                    TestType.Mix => hasWriting ? StudentTestStatus.WaitingForWritingGrading : StudentTestStatus.AutoGradedWaitingForWritingGrading,
                    _ => StudentTestStatus.Submitted
                };

                // Update student test with final values
                studentTest.Mark = hasMCQ ? totalScore : null;
                studentTest.Status = newStatus;

                // Add the new student test to database
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return OperationResult<bool>.Ok(true, "Tạo và nộp bài thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return OperationResult<bool>.Fail("Lỗi hệ thống khi tạo bài: " + ex.Message);
            }
        }

        private async Task<string> GenerateStudentTestIDAsync()
        {
            // Get all StudentTest IDs that start with "ST" and have 4 digits
            var existingIDs = await _dbContext.StudentTest
                .Where(st => st.StudentTestID.StartsWith("ST") && st.StudentTestID.Length == 6)
                .Select(st => st.StudentTestID)
                .ToListAsync();

            // Extract numbers and find the maximum
            var maxNumber = 0;
            foreach (var id in existingIDs)
            {
                var numberPart = id.Substring(2);
                if (int.TryParse(numberPart, out int number))
                {
                    maxNumber = Math.Max(maxNumber, number);
                }
            }

            // Generate next ID
            var nextNumber = maxNumber + 1;
            return $"ST{nextNumber:D4}"; // Format as ST0001, ST0002, etc.
        }

        public async Task<OperationResult<bool>> ValidStudentGetExamAsync(string testEventId, string accountId)
        {
            // 1. Lấy thông tin TestEvent
            var testEvent = await _testEventRepository.GetByIdAsync(testEventId);
            if (testEvent == null)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("Bài kiểm tra"));

            var attemptLimit = testEvent.AttemptLimit;
            var classLessonId = testEvent.ClassLessonID;

            // 2. Lấy Lesson để lấy ClassID
            var lesson = await _lessonRepository.GetLessonByClassLessonIDAsync(classLessonId);
            if (lesson == null)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("Buổi học"));

            var classId = lesson.ClassID;

            // 3. Kiểm tra học sinh có trong lớp hay không 
            var isStudentInClass = await _enrollmentRepository.IsStudentEnrolledAsync(accountId, classId);
            if (!isStudentInClass)
                return OperationResult<bool>.Fail("Học sinh không thuộc lớp này, không thể làm bài.");

            // 4. Đếm số lần học sinh đã làm bài trong StudentTests
            var attemptCount = await _studentTestRepo.CountAttemptsAsync(testEventId, accountId);
            if (attemptLimit.HasValue && attemptCount >= attemptLimit.Value)
                return OperationResult<bool>.Fail("Đã vượt quá số lần làm bài cho phép.");

            // 5. Hợp lệ
            return OperationResult<bool>.Ok(true);
        }

        public async Task<OperationResult<bool>> GradeWritingAnswerAsync(GradeWritingAnswerCommand request)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var writingAnswer = await _dbContext.WritingAnswers
                    .FirstOrDefaultAsync(w => w.WritingAnswerID == request.WritingAnswerID);

                if (writingAnswer == null)
                    return OperationResult<bool>.Fail("Không tìm thấy bài viết.");

                var studentTest = await _dbContext.StudentTest
                    .FirstOrDefaultAsync(s => s.StudentTestID == request.StudentTestID);

                if (studentTest == null)
                    return OperationResult<bool>.Fail("Không tìm thấy bài làm của học sinh.");

                if (studentTest.Status != StudentTestStatus.AutoGradedWaitingForWritingGrading &&
                    studentTest.Status != StudentTestStatus.WaitingForWritingGrading)
                {
                    return OperationResult<bool>.Fail("Chỉ có thể chấm bài viết khi bài làm đang chờ chấm viết.");
                }

                //Cập nhật điểm & feedback
                writingAnswer.Score = request.WritingScore;
                writingAnswer.Feedback = request.Feedback;
                _dbContext.WritingAnswers.Update(writingAnswer);

                studentTest.Mark = (studentTest.Mark ?? 0) + request.WritingScore;
                studentTest.Feedback = request.Feedback;
                studentTest.GradeBy = request.GraderAccountID;
                studentTest.GradeAt = DateTime.Now;
                studentTest.Status = StudentTestStatus.Graded;

                _dbContext.StudentTest.Update(studentTest);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return OperationResult<bool>.Ok(true, "Chấm điểm bài viết thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return OperationResult<bool>.Fail("Lỗi khi chấm điểm bài viết: " + ex.Message);
            }
        }
            public async Task<OperationResult<bool>> ValidateWritingScoreAsync(string testSectionID, decimal writingScore)
        {
            var sectionResult = await _sectionRepo.GetTestSectionByIdAsync(testSectionID);
            if (!sectionResult.Success || sectionResult.Data == null)
                return OperationResult<bool>.Fail("Không tìm thấy phần thi.");

            var totalScore = await _sectionRepo.GetTotalScoreBySectionID(testSectionID);
            if (writingScore > totalScore)
            {
                return OperationResult<bool>.Fail($"Điểm chấm ({writingScore}) vượt quá điểm tối đa ({totalScore}) của phần thi.");
            }

            return OperationResult<bool>.Ok(true);
        }
        public async Task<OperationResult<int>> CountPendingWrittenGradingAsync(string lecturerId)
        {
            var count = await _studentTestRepo.CountPendingWrittenGradingByLecturerAsync(lecturerId);
            return OperationResult<int>.Ok(count, OperationMessages.RetrieveSuccess("số bài chưa được chấm viết"));
        }
    }

}


