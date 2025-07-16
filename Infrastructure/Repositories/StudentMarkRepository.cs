using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StudentMarksRepository : IStudentMarkRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public StudentMarksRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StudentMark> GetByIdAsync(string id)
        {
            return await _dbContext.StudentMarks
                .Include(sm => sm.Account)
                .Include(sm => sm.GradedByAccount)
                .Include(sm => sm.Class)
                .Include(sm => sm.StudentTest)
                .FirstOrDefaultAsync(sm => sm.StudentMarkID == id);
        }

        public async Task<StudentMark> GetByStudentAndAssessmentCriteriaAsync(string studentId, string assessmentCriteriaId, string classId)
        {
            return await _dbContext.StudentMarks
                .FirstOrDefaultAsync(sm => sm.AccountID == studentId &&
                                          sm.AssessmentCriteriaID == assessmentCriteriaId &&
                                          sm.ClassID == classId);
        }

        public async Task<StudentMark> CreateAsync(StudentMark studentMarks)
        {
            _dbContext.StudentMarks.Add(studentMarks);
            await _dbContext.SaveChangesAsync();
            return studentMarks;
        }

        public async Task<StudentMark> UpdateAsync(StudentMark studentMarks)
        {
            _dbContext.StudentMarks.Update(studentMarks);
            await _dbContext.SaveChangesAsync();
            return studentMarks;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _dbContext.StudentMarks.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<StudentMark>> GetByClassIdAsync(string classId)
        {
            return await _dbContext.StudentMarks
                .Where(sm => sm.ClassID == classId)
                .Include(sm => sm.Account) 
                .ToListAsync();
        }

        public async Task<List<StudentMark>> GetByAssessmentCriteriaAndClassAsync(string assessmentCriteriaId, string classId)
        {
            return await _dbContext.StudentMarks
                .Where(sm => sm.AssessmentCriteriaID == assessmentCriteriaId && sm.ClassID == classId)
                .ToListAsync();
        }

        public async Task<List<StudentMark>> GetByStudentIdAsync(string studentId)
        {
            return await _dbContext.StudentMarks
                .Where(sm => sm.AccountID == studentId)
                .Include(sm => sm.Class)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.StudentMarks.CountAsync();
        }

        public async Task<List<StudentMark>> GetByStudentTestIdAsync(string studentTestId)
        {
            return await _dbContext.StudentMarks
                .Where(sm => sm.StudentTestID == studentTestId)
                .ToListAsync();
        }
        //Setup điểm
        public async Task<OperationResult<bool>> SetupStudentMarkByClassIdAsync(string classId)
        {
            var classEntity = await _dbContext.Class
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.ClassID == classId);

            if (classEntity == null)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("lớp học"));

            var studentIds = await _dbContext.ClassEnrollment
                .Where(e => e.ClassID == classId)
                .Select(e => e.StudentID)
                .ToListAsync();

            if (!studentIds.Any())
                return OperationResult<bool>.Fail(OperationMessages.NotFound("học viên"));

            var assessmentCriteriaList = await _dbContext.AssessmentCriteria
                .Where(a => a.SubjectID == classEntity.SubjectID)
                .ToListAsync();

            var now = DateTime.UtcNow;

            // Tìm ID lớn nhất đã có
            var maxIdNumber = _dbContext.StudentMarks
                .Where(sm => sm.StudentMarkID.StartsWith("IM"))
                .Select(sm => sm.StudentMarkID.Substring(2))
                .AsEnumerable()
                .Where(id => int.TryParse(id, out _))
                .Select(int.Parse)
                .DefaultIfEmpty(0)
                .Max();

            int counter = maxIdNumber + 1;

            var studentMarks = studentIds
                .SelectMany(studentId =>
                    assessmentCriteriaList.SelectMany(criteria =>
                    {
                        int count = (criteria.RequiredTestCount ?? 0) > 0
                            ? criteria.RequiredTestCount.Value
                            : 1;

                        return Enumerable.Range(1, count)
                            .Select(attempt => new StudentMark
                            {
                                StudentMarkID = $"IM{(counter++).ToString("D6")}",
                                AccountID = studentId,
                                AssessmentCriteriaID = criteria.AssessmentCriteriaID,
                                ClassID = classId,
                                AssessmentIndex = attempt,
                                Mark = null,
                                IsFinalized = false,
                                CreatedAt = now,
                                UpdatedAt = now
                            });
                    }))
                .ToList();

            await _dbContext.StudentMarks.AddRangeAsync(studentMarks);
            await _dbContext.SaveChangesAsync();

            return OperationResult<bool>.Ok(true, OperationMessages.CreateSuccess("bảng điểm"));
        }
        //Get điểm theo lớp
        public async Task<OperationResult<List<StudentMarkDetailKhoDTO>>> GetStudentMarkDetailDTOByClassIdAsync(string classId)
        {
            var studentMarks = await _dbContext.StudentMarks
                .Where(sm => sm.ClassID == classId)
                .Include(sm => sm.AssessmentCriteria)
                .Include(sm => sm.Account)
                .Include(sm => sm.StudentTest)
                    .ThenInclude(st => st.Student)
                .ToListAsync();

            var result = studentMarks
                .GroupBy(sm => new { sm.AssessmentCriteria.Category, sm.AssessmentIndex })
                .Select(g => new StudentMarkDetailKhoDTO
                {
                    AssessmentCategory = (AssessmentCategory)g.Key.Category,
                    AssessmentIndex = g.Key.AssessmentIndex,
                    StudentMarks = g.Select(sm => new StudentMarkItem
                    {
                        StudentMarkID = sm.StudentMarkID,
                        StudentName = sm.Account?.LastName + " " + sm.Account?.FirstName,
                        Mark = sm.Mark,
                        Comment = sm.Comment,
                        StudentTestID = sm.StudentTestID,
                    }).ToList()
                })
                .ToList();

            return OperationResult<List<StudentMarkDetailKhoDTO>>.Ok(result, OperationMessages.RetrieveSuccess("bảng điểm"));
        }

        public async Task<List<StudentMark>> GetMarksByStudentAndClassAsync(string studentID, string classID)
        {
            return await _dbContext.StudentMarks
                .Where(sm => sm.AccountID == studentID && sm.ClassID == classID)
                .Include(sm => sm.AssessmentCriteria)
                .ToListAsync();
        }
        public async Task<List<StudentMark>> GetMarksByClassAsync(string classId)
        {
            return await _dbContext.StudentMarks
                .Where(sm => sm.ClassID == classId)
                .Include(sm => sm.AssessmentCriteria)
                .ToListAsync();
        }

    }
}