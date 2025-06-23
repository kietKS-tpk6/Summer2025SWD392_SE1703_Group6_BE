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

        private readonly HangulLearningSystemDbContext _dbContext;

        public StudentTestService(
            IStudentTestRepository studentTestRepo,
            IQuestionRepository questionRepo,
            ITestSectionRepository sectionRepo,
            IMCQOptionRepository optionRepo,
            IMCQAnswerRepository mcqAnswerRepo,
            IWritingAnswerRepository writingAnswerRepo,
            HangulLearningSystemDbContext dbContext,
            ITestRepository testRepository)
        {
            _studentTestRepo = studentTestRepo;
            _questionRepo = questionRepo;
            _sectionRepo = sectionRepo;
            _optionRepo = optionRepo;
            _mcqAnswerRepo = mcqAnswerRepo;
            _writingAnswerRepo = writingAnswerRepo;
            _dbContext = dbContext;
            _testRepo = testRepository;
        }

        public async Task<OperationResult<bool>> ValidateStudentTestExistsAsync(string studentTestID)
        {
            var exists = await _studentTestRepo.ExistsAsync(studentTestID);
            return exists ? OperationResult<bool>.Ok(true) : OperationResult<bool>.Fail("StudentTest không tồn tại.");
        }

        public async Task<OperationResult<bool>> SubmitStudentTestAsync(
     string studentTestID,
     string testID,
     List<SectionAnswerDTO> sectionAnswers)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                decimal totalScore = 0;
                bool hasWriting = false;
                bool hasMCQ = false;

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
                            var essay = answer.Answers.FirstOrDefault() ?? "";
                            await _writingAnswerRepo.SaveAnswerAsync(studentTestID, answer.QuestionID, essay);
                        }
                        else
                        {
                            await _mcqAnswerRepo.SaveAnswerAsync(studentTestID, answer.QuestionID, answer.Answers);

                            var correctIDs = await _optionRepo.GetCorrectOptionIDsAsync(answer.QuestionID);
                            bool correct = correctIDs.OrderBy(x => x).SequenceEqual(answer.Answers.OrderBy(x => x));
                            if (correct)
                            {
                                var q = await _questionRepo.GetByIdAsync(answer.QuestionID);
                                totalScore += q?.Score ?? 0;
                            }
                        }
                    }
                }

                var studentTest = await _studentTestRepo.GetByIdAsync(studentTestID);
                if (studentTest == null)
                    return OperationResult<bool>.Fail("Không tìm thấy bài làm.");

                var test = await _testRepo.GetTestByIdAsync(testID);
                if (test == null)
                    return OperationResult<bool>.Fail("Không tìm thấy bài kiểm tra.");

                studentTest.SubmitTime = DateTime.Now;
                studentTest.Score = hasMCQ ? totalScore : null;

                switch (test.Data.TestType)
                {
                    case TestType.MCQ:
                        studentTest.Status = StudentTestStatus.Graded;
                        break;

                    case TestType.Writing:
                        studentTest.Status = StudentTestStatus.WaitingForGrading;
                        break;

                    case TestType.Mix:
                        studentTest.Status = hasWriting
                            ? StudentTestStatus.WaitingForGrading
                            : StudentTestStatus.AutoGraded;
                        break;

                    default:
                        studentTest.Status = StudentTestStatus.Submitted;
                        break;
                }

                await _studentTestRepo.UpdateAsync(studentTest);
                await transaction.CommitAsync();

                return OperationResult<bool>.Ok(true, "Nộp bài và xử lý thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return OperationResult<bool>.Fail("Lỗi hệ thống khi lưu bài: " + ex.Message);
            }
        }

    }

}
