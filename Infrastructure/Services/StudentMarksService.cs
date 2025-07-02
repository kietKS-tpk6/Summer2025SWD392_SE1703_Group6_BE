using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;

namespace Infrastructure.Services
{
    public class StudentMarksService : IStudentMarksService
    {
        private readonly IStudentMarkRepository _studentMarksRepository;
        private readonly IStudentTestRepository _studentTestRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly ITestSectionRepository _testSectionRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestEventRepository _testEventRepository;
        private readonly ISyllabusScheduleTestRepository _syllabusScheduleTestsRepository;
        private readonly ILessonRepository _lessonRepository;


        public StudentMarksService(
            IStudentMarkRepository studentMarksRepository,
            IStudentTestRepository studentTestRepository,
            IAssessmentCriteriaRepository assessmentCriteriaRepository,
            ITestSectionRepository testSectionRepository,
            ITestRepository testRepository,
            ITestEventRepository testEventRepository,
            ISyllabusScheduleTestRepository syllabusScheduleTestsRepository,
            ILessonRepository lessonRepository)
        {
            _studentMarksRepository = studentMarksRepository;
            _studentTestRepository = studentTestRepository;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _testSectionRepository = testSectionRepository;
            _testRepository = testRepository;
            _testEventRepository = testEventRepository;
            _syllabusScheduleTestsRepository = syllabusScheduleTestsRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task<OperationResult<GetTestScoresDTO>> GetTestScoresByTestIdAsync(string testId)
        {
            try
            {
                var testSections = await _testSectionRepository.GetTestSectionsByTestIdAsync(testId);

                if (testSections == null || !testSections.Data.Any())
                {
                    return OperationResult<GetTestScoresDTO>.Fail("No test sections found for this test");
                }

                var result = new GetTestScoresDTO
                {
                    TestId = testId,
                    TestSections = testSections.Data.Select(ts => new TestSectionScoreDTO
                    {
                        TestSectionId = ts.TestSectionID,
                        Context = ts.Context,
                        Score = ts.Score
                    }).ToList(),
                    TotalScore = testSections.Data.Sum(ts => ts.Score)
                };

                return OperationResult<GetTestScoresDTO>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<GetTestScoresDTO>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<string>> CreateStudentMarkFromStudentTestAsync(string studentTestId)
        {
            try
            {
                Console.WriteLine($"Starting CreateStudentMarkFromStudentTestAsync with studentTestId: {studentTestId}");

                // Get StudentTest with related data
                var studentTest = await _studentTestRepository.GetByIdAsync(studentTestId);
                if (studentTest == null)
                {
                    return OperationResult<string>.Fail("Student test not found");
                }

                Console.WriteLine($"StudentTest found: {studentTest.StudentTestID}, Mark: {studentTest.Mark}");

                if (studentTest.Mark == null)
                {
                    return OperationResult<string>.Fail("Student test has no mark to transfer");
                }

                // Load TestEvent if not included
                if (studentTest.TestEvent == null)
                {
                    Console.WriteLine("TestEvent is null, trying to load separately");
                    var testEvent = await _testEventRepository.GetByIdAsync(studentTest.TestEventID);
                    if (testEvent == null)
                    {
                        return OperationResult<string>.Fail("Test event not found");
                    }
                    studentTest.TestEvent = testEvent;
                }

                Console.WriteLine($"TestEvent found: {studentTest.TestEvent.TestEventID}, ClassLessonID: {studentTest.TestEvent.ClassLessonID}");

                // Get ClassID through the chain
                string classId = null;
                if (!string.IsNullOrEmpty(studentTest.TestEvent.ClassLessonID))
                {
                    var lessonDetail = await _lessonRepository.GetLessonDetailByLessonIDAsync(studentTest.TestEvent.ClassLessonID);
                    if (lessonDetail != null && !string.IsNullOrEmpty(lessonDetail.ClassID))
                    {
                        classId = lessonDetail.ClassID;
                        Console.WriteLine($"ClassID found: {classId}");
                    }
                    else
                    {
                        return OperationResult<string>.Fail("Could not find ClassID from the lesson associated with this test event");
                    }
                }
                else
                {
                    return OperationResult<string>.Fail("Test event does not have an associated class lesson");
                }

                // Get SyllabusScheduleTest
                var syllabusScheduleTest = await _syllabusScheduleTestsRepository.GetByScheduleTestIdAsync(studentTest.TestEvent.ScheduleTestID);
                if (syllabusScheduleTest == null)
                {
                    return OperationResult<string>.Fail("Syllabus schedule test not found");
                }

                Console.WriteLine($"SyllabusScheduleTest found: AssessmentCriteriaID: {syllabusScheduleTest.AssessmentCriteriaID}");

                // Get AssessmentCriteria
                var assessmentCriteria = await _assessmentCriteriaRepository.GetByIdAsync(syllabusScheduleTest.AssessmentCriteriaID);
                if (assessmentCriteria == null || !assessmentCriteria.Success)
                {
                    return OperationResult<string>.Fail("Assessment criteria not found");
                }

                Console.WriteLine($"AssessmentCriteria found: {assessmentCriteria.Data.Category}");

                // Check if StudentMark already exists
                var existingStudentMark = await _studentMarksRepository.GetByStudentAndAssessmentCriteriaAsync(
                    studentTest.StudentID, syllabusScheduleTest.AssessmentCriteriaID, classId);

                if (existingStudentMark != null)
                {
                    return OperationResult<string>.Fail("Student mark already exists for this assessment criteria");
                }

                // Validate all IDs before creating StudentMark
                Console.WriteLine($"Validating IDs:");
                Console.WriteLine($"- StudentID (AccountID): {studentTest.StudentID}");
                Console.WriteLine($"- AssessmentCriteriaID: {syllabusScheduleTest.AssessmentCriteriaID}");
                Console.WriteLine($"- ClassID: {classId}");
                Console.WriteLine($"- StudentTestID: {studentTestId}");

                // Create new StudentMark with proper ID handling
                var newStudentMarkId = GenerateStudentMarkId();
                var newStudentMark = new StudentMark
                {
                    StudentMarkID = newStudentMarkId,
                    AccountID = studentTest.StudentID,
                    AssessmentCriteriaID = syllabusScheduleTest.AssessmentCriteriaID,
                    ClassID = classId,
                    Mark = studentTest.Mark,
                    StudentTestID = studentTestId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsFinalized = assessmentCriteria.Data.Category == AssessmentCategory.Midterm ||
                                 assessmentCriteria.Data.Category == AssessmentCategory.Final
                };

                Console.WriteLine($"Creating StudentMark with ID: {newStudentMarkId}");

                try
                {
                    await _studentMarksRepository.CreateAsync(newStudentMark);
                    Console.WriteLine("StudentMark created successfully");
                    return OperationResult<string>.Ok(newStudentMark.StudentMarkID, "Student mark created successfully from student test");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating StudentMark: {ex.Message}");
                    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

                    // Log detailed information about the entity being saved
                    Console.WriteLine($"Entity details:");
                    Console.WriteLine($"- StudentMarkID: {newStudentMark.StudentMarkID} (Type: {newStudentMark.StudentMarkID?.GetType()})");
                    Console.WriteLine($"- AccountID: {newStudentMark.AccountID} (Type: {newStudentMark.AccountID?.GetType()})");
                    Console.WriteLine($"- AssessmentCriteriaID: {newStudentMark.AssessmentCriteriaID} (Type: {newStudentMark.AssessmentCriteriaID?.GetType()})");
                    Console.WriteLine($"- ClassID: {newStudentMark.ClassID} (Type: {newStudentMark.ClassID?.GetType()})");
                    Console.WriteLine($"- StudentTestID: {newStudentMark.StudentTestID} (Type: {newStudentMark.StudentTestID?.GetType()})");

                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateStudentMarkFromStudentTestAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return OperationResult<string>.Fail($"Error creating student mark: {ex.Message}");
            }
        }

        public async Task<OperationResult<string>> UpdateStudentMarksFromStudentTestAsync(string studentTestId, string assessmentCriteriaId, string classId)
        {
            try
            {
                var studentTest = await _studentTestRepository.GetByIdAsync(studentTestId);
                if (studentTest == null)
                {
                    return OperationResult<string>.Fail("Student test not found");
                }

                var assessmentCriteria = await _assessmentCriteriaRepository.GetByIdAsync(assessmentCriteriaId);
                if (assessmentCriteria == null)
                {
                    return OperationResult<string>.Fail("Assessment criteria not found");
                }

                // Calculate total score from test sections if student test mark is null
                decimal finalMark = studentTest.Mark ?? 0;

                if (studentTest.Mark == null && studentTest.TestEvent?.TestID != null)
                {
                    var testSections = await _testSectionRepository.GetTestSectionsByTestIdAsync(studentTest.TestEvent.TestID);
                    finalMark = testSections.Data.Sum(ts => ts.Score);
                }

                // Check if student marks already exists
                var existingStudentMarks = await _studentMarksRepository.GetByStudentAndAssessmentCriteriaAsync(
                    studentTest.StudentID, assessmentCriteriaId, classId);

                if (existingStudentMarks != null)
                {
                    // Check if it's midterm or final - cannot be changed
                    if (assessmentCriteria.Data.Category == AssessmentCategory.Midterm ||
                        assessmentCriteria.Data.Category == AssessmentCategory.Final)
                    {
                        return OperationResult<string>.Fail("Cannot update marks for Midterm or Final assessments - they are automatically calculated from test results");
                    }

                    // Update existing record
                    existingStudentMarks.Mark = finalMark;
                    existingStudentMarks.UpdatedAt = DateTime.UtcNow;
                    existingStudentMarks.StudentTestID = studentTestId;

                    await _studentMarksRepository.UpdateAsync(existingStudentMarks);
                    return OperationResult<string>.Ok(existingStudentMarks.StudentMarkID, "Student marks updated successfully");
                }
                else
                {
                    // Create new record
                    var newStudentMarks = new StudentMark
                    {
                        StudentMarkID = GenerateStudentMarkId(),
                        AccountID = studentTest.StudentID,
                        AssessmentCriteriaID = assessmentCriteriaId,
                        ClassID = classId,
                        Mark = finalMark,
                        StudentTestID = studentTestId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsFinalized = assessmentCriteria.Data.Category == AssessmentCategory.Midterm ||
                                     assessmentCriteria.Data.Category == AssessmentCategory.Final
                    };

                    await _studentMarksRepository.CreateAsync(newStudentMarks);
                    return OperationResult<string>.Ok(newStudentMarks.StudentMarkID, "Student marks created successfully");
                }
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<bool>> UpdateStudentMarksByLecturerAsync(string studentMarkId, decimal mark, string comment, string lecturerId)
        {
            try
            {
                var studentMarks = await _studentMarksRepository.GetByIdAsync(studentMarkId);
                if (studentMarks == null)
                {
                    return OperationResult<bool>.Fail("Student marks not found");
                }

                var assessmentCriteria = await _assessmentCriteriaRepository.GetByIdAsync(studentMarks.AssessmentCriteriaID);
                if (assessmentCriteria == null)
                {
                    return OperationResult<bool>.Fail("Assessment criteria not found");
                }

                // Check if it's midterm or final - cannot be changed by lecturer
                if (assessmentCriteria.Data.Category == AssessmentCategory.Midterm ||
                    assessmentCriteria.Data.Category == AssessmentCategory.Final)
                {
                    return OperationResult<bool>.Fail("Cannot manually update marks for Midterm or Final assessments - they are automatically calculated from test results");
                }

                // Update marks
                studentMarks.Mark = mark;
                studentMarks.Comment = comment;
                studentMarks.GradedBy = lecturerId;
                studentMarks.GradedAt = DateTime.UtcNow;
                studentMarks.UpdatedAt = DateTime.UtcNow;

                await _studentMarksRepository.UpdateAsync(studentMarks);
                return OperationResult<bool>.Ok(true, "Student marks updated by lecturer successfully");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<BatchUpdateResultDTO>> BatchUpdateStudentMarksFromStudentTestsAsync(List<StudentTestUpdateDTO> studentTests, string assessmentCriteriaId, string classId)
        {
            var result = new BatchUpdateResultDTO();
            int successCount = 0;

            try
            {
                var assessmentCriteria = await _assessmentCriteriaRepository.GetByIdAsync(assessmentCriteriaId);
                if (assessmentCriteria == null)
                {
                    return OperationResult<BatchUpdateResultDTO>.Fail("Assessment criteria not found");
                }

                foreach (var studentTestDto in studentTests)
                {
                    try
                    {
                        var studentTest = await _studentTestRepository.GetByIdAsync(studentTestDto.StudentTestId);
                        if (studentTest == null)
                        {
                            result.Errors.Add($"Student test {studentTestDto.StudentTestId} not found");
                            continue;
                        }

                        // Calculate total score from test sections if student test mark is null
                        decimal finalMark = studentTest.Mark ?? 0;

                        if (studentTest.Mark == null && studentTest.TestEvent?.TestID != null)
                        {
                            var testSections = await _testSectionRepository.GetTestSectionsByTestIdAsync(studentTest.TestEvent.TestID);
                            finalMark = testSections.Data.Sum(ts => ts.Score);
                        }

                        // Check if student marks already exists
                        var existingStudentMarks = await _studentMarksRepository.GetByStudentAndAssessmentCriteriaAsync(
                            studentTest.StudentID, assessmentCriteriaId, classId);

                        if (existingStudentMarks != null)
                        {
                            // Update existing record
                            existingStudentMarks.Mark = finalMark;
                            existingStudentMarks.UpdatedAt = DateTime.UtcNow;
                            existingStudentMarks.StudentTestID = studentTestDto.StudentTestId;

                            await _studentMarksRepository.UpdateAsync(existingStudentMarks);
                        }
                        else
                        {
                            // Create new record
                            var newStudentMarks = new StudentMark
                            {
                                StudentMarkID = GenerateStudentMarkId(),
                                AccountID = studentTest.StudentID,
                                AssessmentCriteriaID = assessmentCriteriaId,
                                ClassID = classId,
                                Mark = finalMark,
                                StudentTestID = studentTestDto.StudentTestId,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsFinalized = assessmentCriteria.Data.Category == AssessmentCategory.Midterm ||
                                             assessmentCriteria.Data.Category == AssessmentCategory.Final
                            };

                            await _studentMarksRepository.CreateAsync(newStudentMarks);
                        }

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Error updating student test {studentTestDto.StudentTestId}: {ex.Message}");
                    }
                }

                result.UpdatedCount = successCount;

                if (successCount > 0)
                {
                    return OperationResult<BatchUpdateResultDTO>.Ok(result, $"Successfully updated {successCount} out of {studentTests.Count} student marks");
                }
                else
                {
                    return OperationResult<BatchUpdateResultDTO>.Fail("No student marks were updated");
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
                return OperationResult<BatchUpdateResultDTO>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<List<StudentMarkDTO>>> GetStudentMarksByStudentIdAsync(string studentId)
        {
            try
            {
                var studentMarks = await _studentMarksRepository.GetByStudentIdAsync(studentId);

                var result = studentMarks.Select(sm => new StudentMarkDTO
                {
                    StudentMarkID = sm.StudentMarkID,
                    AccountID = sm.AccountID,
                    AssessmentCriteriaID = sm.AssessmentCriteriaID,
                    Mark = sm.Mark,
                    Comment = sm.Comment,
                    GradedBy = sm.GradedBy,
                    GradedAt = sm.GradedAt,
                    IsFinalized = sm.IsFinalized,
                    CreatedAt = sm.CreatedAt,
                    UpdatedAt = sm.UpdatedAt,
                    ClassID = sm.ClassID
                }).ToList();

                return OperationResult<List<StudentMarkDTO>>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<List<StudentMarkDTO>>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<List<StudentMarkDTO>>> GetStudentMarksByClassAndAssessmentAsync(string classId, string assessmentCriteriaId)
        {
            try
            {
                var studentMarks = await _studentMarksRepository.GetByAssessmentCriteriaAndClassAsync(assessmentCriteriaId, classId);

                var result = studentMarks.Select(sm => new StudentMarkDTO
                {
                    StudentMarkID = sm.StudentMarkID,
                    AccountID = sm.AccountID,
                    AssessmentCriteriaID = sm.AssessmentCriteriaID,
                    Mark = sm.Mark,
                    Comment = sm.Comment,
                    GradedBy = sm.GradedBy,
                    GradedAt = sm.GradedAt,
                    IsFinalized = sm.IsFinalized,
                    CreatedAt = sm.CreatedAt,
                    UpdatedAt = sm.UpdatedAt,
                    ClassID = sm.ClassID
                }).ToList();

                return OperationResult<List<StudentMarkDTO>>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<List<StudentMarkDTO>>.Fail(ex.Message);
            }
        }

        private async Task<decimal> GetFinalMarkFromStudentTest(StudentTest studentTest)
        {
            // If student test already has mark, use it
            if (studentTest.Mark.HasValue)
            {
                return studentTest.Mark.Value;
            }

            // Otherwise calculate from test sections
            if (studentTest.TestEvent?.TestID != null)
            {
                var testSections = await _testSectionRepository.GetTestSectionsByTestIdAsync(studentTest.TestEvent.TestID);
                if (testSections.Success && testSections.Data.Any())
                {
                    return testSections.Data.Sum(ts => ts.Score);
                }
            }

            return 0;
        }
        public async Task<OperationResult<bool>> DeleteStudentMarkAsync(string studentMarkId)
        {
            try
            {
                var studentMark = await _studentMarksRepository.GetByIdAsync(studentMarkId);
                if (studentMark == null)
                {
                    return OperationResult<bool>.Fail("Student mark not found");
                }

                // Check if the mark is finalized (Midterm or Final assessments)
                var assessmentCriteria = await _assessmentCriteriaRepository.GetByIdAsync(studentMark.AssessmentCriteriaID);
                if (assessmentCriteria.Success &&
                    (assessmentCriteria.Data.Category == AssessmentCategory.Midterm ||
                     assessmentCriteria.Data.Category == AssessmentCategory.Final))
                {
                    return OperationResult<bool>.Fail("Cannot delete marks for Midterm or Final assessments");
                }

                // Check if the mark is finalized
                if (studentMark.IsFinalized)
                {
                    return OperationResult<bool>.Fail("Cannot delete finalized student marks");
                }

                await _studentMarksRepository.DeleteAsync(studentMarkId);
                return OperationResult<bool>.Ok(true, "Student mark deleted successfully");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail($"Error deleting student mark: {ex.Message}");
            }
        }

        private static int _studentMarkCounter = 0;
        private string GenerateStudentMarkId()
        {
            _studentMarkCounter++;
            string number = _studentMarkCounter.ToString("D6");
            return $"IM{number}";
        }
    }
}