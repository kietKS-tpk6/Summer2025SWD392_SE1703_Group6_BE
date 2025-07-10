using Application.Common.Constants;
using Domain.Entities;
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
            if(!studentIds.Any())
            {
                return OperationResult<bool>.Fail(OperationMessages.NotFound("học viên"));
            }
            var assessmentCriteriaList = await _dbContext.AssessmentCriteria
                .Where(a => a.SubjectID == classEntity.SubjectID)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var maxIdNumber = _dbContext.StudentMarks
            .Where(sm => sm.StudentMarkID.StartsWith("IM"))
            .Select(sm => sm.StudentMarkID.Substring(2))
            .AsEnumerable() 
            .Where(id => int.TryParse(id, out var _))
            .Select(id => int.Parse(id))
            .DefaultIfEmpty(0)
            .Max();


            int counter = maxIdNumber + 1;
            var studentMarks = studentIds
                .SelectMany(studentId => assessmentCriteriaList, (studentId, criteria) => new StudentMark
                {
                    StudentMarkID = $"IM{(counter++).ToString("D6")}",
                    AccountID = studentId,
                    AssessmentCriteriaID = criteria.AssessmentCriteriaID,
                    ClassID = classId,
                    IsFinalized = false,
                    CreatedAt = now,
                    UpdatedAt = now
                }).ToList();

            await _dbContext.StudentMarks.AddRangeAsync(studentMarks);
            await _dbContext.SaveChangesAsync();

            return OperationResult<bool>.Ok(true, "Khởi tạo bảng điểm thành công");
        }

    }
}