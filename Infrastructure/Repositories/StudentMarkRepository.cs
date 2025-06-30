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
        private readonly HangulLearningSystemDbContext _context;

        public StudentMarksRepository(HangulLearningSystemDbContext context)
        {
            _context = context;
        }

        public async Task<StudentMark> GetByIdAsync(string id)
        {
            return await _context.StudentMarks
                .Include(sm => sm.AccountID)
                .Include(sm => sm.AssessmentCriteriaID)
                .Include(sm => sm.GradedByAccount)
                .Include(sm => sm.Class)
                .FirstOrDefaultAsync(sm => sm.StudentMarkID == id);
        }

        public async Task<StudentMark> GetByStudentAndAssessmentCriteriaAsync(string studentId, string assessmentCriteriaId, string classId)
        {
            return await _context.StudentMarks
                .FirstOrDefaultAsync(sm => sm.AccountID == studentId &&
                                          sm.AssessmentCriteriaID == assessmentCriteriaId &&
                                          sm.ClassID == classId);
        }

        public async Task<StudentMark> CreateAsync(StudentMark studentMarks)
        {
            _context.StudentMarks.Add(studentMarks);
            await _context.SaveChangesAsync();
            return studentMarks;
        }

        public async Task<StudentMark> UpdateAsync(StudentMark studentMarks)
        {
            _context.StudentMarks.Update(studentMarks);
            await _context.SaveChangesAsync();
            return studentMarks;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.StudentMarks.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<StudentMark>> GetByAssessmentCriteriaAndClassAsync(string assessmentCriteriaId, string classId)
        {
            return await _context.StudentMarks
                .Where(sm => sm.AssessmentCriteriaID == assessmentCriteriaId && sm.ClassID == classId)
                .Include(sm => sm.AccountID)
                .ToListAsync();
        }

        public async Task<List<StudentMark>> GetByStudentIdAsync(string studentId)
        {
            return await _context.StudentMarks
                .Where(sm => sm.AccountID == studentId)
                .Include(sm => sm.AssessmentCriteriaID)
                .Include(sm => sm.Class)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.StudentMarks.CountAsync();
        }

        public async Task<List<StudentMark>> GetByStudentTestIdAsync(string studentTestId)
        {
            return await _context.StudentMarks
                .Where(sm => sm.StudentTestID == studentTestId)
                .ToListAsync();
        }

    }
}