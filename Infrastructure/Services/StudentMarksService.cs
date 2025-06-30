using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class StudentMarksService : IStudentMarksService
    {
        private readonly IStudentMarksRepository _studentMarksRepository;
        private readonly IStudentTestRepository _studentTestRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly ITestSectionRepository _testSectionRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestEventRepository _testEventRepository;

        public StudentMarksService(
            IStudentMarksRepository studentMarksRepository,
            IStudentTestRepository studentTestRepository,
            IAssessmentCriteriaRepository assessmentCriteriaRepository,
            ITestSectionRepository testSectionRepository,
            ITestRepository testRepository,
            ITestEventRepository testEventRepository)
        {
            _studentMarksRepository = studentMarksRepository;
            _studentTestRepository = studentTestRepository;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _testSectionRepository = testSectionRepository;
            _testRepository = testRepository;
            _testEventRepository = testEventRepository;
        }

        public async Task<OperationResult<GetTestScoresDTO>> GetTestScoresByTestIdAsync(string testId)
        {
            try
            {
                var testSections = await _testSectionRepository.GetTestSectionsByTestIdAsync(testId);

                if (testSections == null || !testSections.Any())
                {
                    return OperationResult<GetTestScoresDTO>.Fail("No test sections found for this test");
                }

                var result = new GetTestScoresDTO
                {
                    TestId = testId,
                    TestSections = testSections.Select(ts => new TestSectionScoreDto
                    {
                        TestSectionId = ts.TestSectionID,
                        Context = ts.Context,
                        Score = ts.Score
                    }).ToList(),
                    TotalScore = testSections.Sum(ts => ts.Score)
                };

                return OperationResult<GetTestScoresDTO>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<GetTestScoresDTO>.Fail(ex.Message);
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
                    finalMark = testSections.Sum(ts => ts.Score);
                }

                // Check if student marks already exists
                var existingStudentMarks = await _studentMarksRepository.GetByStudentAndAssessmentCriteriaAsync(
                    studentTest.StudentID, assessmentCriteriaId, classId);

                if (existingStudentMarks != null)
                {
                    // Check if it's midterm or final - cannot be changed
                    if (assessmentCriteria.Category == AssessmentCategory.Midterm ||
                        assessmentCriteria.Category == AssessmentCategory.Final)
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
                    var newStudentMarks = new StudentMarks
                    {
                        StudentMarkID = GenerateStudentMarkId(),
                        AccountID = studentTest.StudentID,
                        AssessmentCriteriaID = assessmentCriteriaId,
                        ClassID = classId,
                        Mark = finalMark,
                        StudentTestID = studentTestId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsFinalized = assessmentCriteria.Category == AssessmentCategory.Midterm ||
                                     assessmentCriteria.Category == AssessmentCategory.Final
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
                if (assessmentCriteria.Category == AssessmentCategory.Midterm ||
                    assessmentCriteria.Category == AssessmentCategory.Final)
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
                            finalMark = testSections.Sum(ts => ts.Score);
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
                            var newStudentMarks = new StudentMarks
                            {
                                StudentMarkID = GenerateStudentMarkId(),
                                AccountID = studentTest.StudentID,
                                AssessmentCriteriaID = assessmentCriteriaId,
                                ClassID = classId,
                                Mark = finalMark,
                                StudentTestID = studentTestDto.StudentTestId,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsFinalized = assessmentCriteria.Category == AssessmentCategory.Midterm ||
                                             assessmentCriteria.Category == AssessmentCategory.Final
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

        private string GenerateStudentMarkId()
        {
            // Tạo ID 6 ký tự theo format của hệ thống
            var timestamp = DateTime.Now.ToString("yyMMddHHmmss");
            return timestamp.Substring(0, 6);
        }
    }
}