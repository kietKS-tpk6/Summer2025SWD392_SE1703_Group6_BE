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

    }
}