using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public EnrollmentRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateEnrollmentAsync(ClassEnrollment enrollment)
        {
            try
            {
                _dbContext.ClassEnrollment.Add(enrollment);
                await _dbContext.SaveChangesAsync();
                return "Enrollment created successfully";
            }
            catch (Exception ex)
            {
                return $"Error creating enrollment: {ex.Message}";
            }
        }

        public async Task<ClassEnrollment> GetEnrollmentByIdAsync(string enrollmentId)
        {
            return await _dbContext.ClassEnrollment
                .Include(e => e.Student)
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.ClassEnrollmentID == enrollmentId);
        }

        public async Task<List<ClassEnrollment>> GetEnrollmentsByStudentIdAsync(string studentId)
        {
            return await _dbContext.ClassEnrollment
                .Include(e => e.Class)
                    .ThenInclude(c => c.Subject)
                .Include(e => e.Class)
                    .ThenInclude(c => c.Lecturer)
                .Where(e => e.StudentID == studentId)
                .OrderByDescending(e => e.EnrolledDate)
                .ToListAsync();
        }

        public async Task<List<ClassEnrollment>> GetEnrollmentsByClassIdAsync(string classId)
        {
            return await _dbContext.ClassEnrollment
                .Include(e => e.Student)
                .Where(e => e.ClassID == classId)
                .OrderBy(e => e.EnrolledDate)
                .ToListAsync();
        }

        public async Task<bool> IsStudentEnrolledAsync(string studentId, string classId)
        {
            return await _dbContext.ClassEnrollment
                .AnyAsync(e => e.StudentID == studentId && e.ClassID == classId);
        }

        public async Task<int> GetClassCurrentEnrollmentsAsync(string classId)
        {
            return await _dbContext.ClassEnrollment
                .CountAsync(e => e.ClassID == classId && e.Status == Domain.Enums.EnrollmentStatus.Actived);
        }

        public async Task<int> GetTotalEnrollmentsCountAsync()
        {
            return await _dbContext.ClassEnrollment.CountAsync();
        }

        public async Task<List<Lesson>> GetLessonsByStudentIdAsync(string studentId)
        {
            return await _dbContext.Lesson
                .Include(l => l.Class)
                .Include(l => l.SyllabusSchedule)
                .Where(l => _dbContext.ClassEnrollment
                    .Any(e => e.StudentID == studentId &&
                             e.ClassID == l.ClassID &&
                             e.Status == Domain.Enums.EnrollmentStatus.Actived))
                .Where(l => l.IsActive)
                .ToListAsync();
        }

        public async Task<List<Lesson>> GetLessonsByClassIdAsync(string classId)
        {
            return await _dbContext.Lesson
                .Include(l => l.SyllabusSchedule)
                .Where(l => l.ClassID == classId && l.IsActive)
                .ToListAsync();
        }
        public async Task<int> CountActiveStudentsOfLecturerAsync(string lecturerId)
        {
            return await (
                from ce in _dbContext.ClassEnrollment
                join c in _dbContext.Class on ce.ClassID equals c.ClassID
                join a in _dbContext.Accounts on ce.StudentID equals a.AccountID
                where c.LecturerID == lecturerId
                      && c.Status == ClassStatus.Ongoing
                      && ce.Status == EnrollmentStatus.Actived
                      && a.Status == AccountStatus.Active
                select ce
            ).CountAsync();
        }
        public async Task<bool> UpdateEnrollmentAsync(ClassEnrollment enrollment)
        {
            try
            {
                _dbContext.ClassEnrollment.Update(enrollment);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}