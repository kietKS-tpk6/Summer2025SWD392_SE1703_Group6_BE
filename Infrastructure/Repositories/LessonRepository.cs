using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using Application.DTOs;
namespace Infrastructure.Repositories
{
    public class LessonRepository : ILessonRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public LessonRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateAsync(Lesson lesson)
        {
            _dbContext.Lesson.Add(lesson);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<int> CountAsync()
        {
            return await _dbContext.Lesson.CountAsync();
        }
        public async Task<bool> UpdateAsync(Lesson lesson)
        {
            _dbContext.Lesson.Update(lesson);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _dbContext.Lesson.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            entity.IsActive = false;
            _dbContext.Lesson.Update(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<List<Lesson>> GetLessonsByClassIDAsync(string classID)
        {
            return await _dbContext.Lesson
                .Include(cl => cl.SyllabusSchedule)
                .Include(cl => cl.Class)
                    .ThenInclude(c => c.Subject)
                .Where(cl => cl.ClassID == classID)
                .ToListAsync();
        }
        public async Task<List<Lesson>> GetLessonsByStudentIDAsync(string studentID)
        {
            return await _dbContext.Lesson
                .Include(cl => cl.SyllabusSchedule)
                .Include(cl => cl.Class)
                    .ThenInclude(c => c.Subject)
                .Where(cl =>
                    cl.IsActive &&
                    _dbContext.ClassEnrollment.Any(enroll =>
                        enroll.ClassID == cl.ClassID &&
                        enroll.StudentID == studentID &&
                        enroll.Status == EnrollmentStatus.Actived))
                .OrderBy(cl => cl.StartTime)
                .ToListAsync();
        }
        public async Task<List<Lesson>> GetLessonsByLecturerIDAsync(string lecturerID)
        {
            return await _dbContext.Lesson
                .Include(cl => cl.SyllabusSchedule)
                .Include(cl => cl.Class)
                    .ThenInclude(c => c.Subject)
                .Where(cl =>
                    cl.IsActive &&
                    cl.LecturerID == lecturerID &&
                    (cl.Class.Status == ClassStatus.Open || cl.Class.Status == ClassStatus.Ongoing))
                .OrderBy(cl => cl.StartTime)
                .ToListAsync();
        }
        public async Task<LessonDetailDTO> GetLessonDetailByLessonIDAsync(string classLessonID)
        {
            var lesson = await _dbContext.Lesson
                .Include(l => l.Class)
                    .ThenInclude(c => c.Subject)
                .Include(l => l.SyllabusSchedule)
                .Include(l => l.Lecturer)
                .FirstOrDefaultAsync(l => l.ClassLessonID == classLessonID);

            if (lesson == null) return null;

            return new LessonDetailDTO
            {
                ClassLessonID = lesson.ClassLessonID,
                ClassName = lesson.Class?.ClassName,
                SubjectName = lesson.Class?.Subject?.SubjectName,
                LecturerName = lesson.Lecturer?.Fullname,
                LecturerID = lesson.LecturerID,
                LessonTitle = lesson.SyllabusSchedule?.LessonTitle,
                Content = lesson.SyllabusSchedule?.Content,
                Resources = lesson.SyllabusSchedule?.Resources,
                DateTime = lesson.StartTime,
                EndTime = lesson.StartTime.AddMinutes(lesson.SyllabusSchedule?.DurationMinutes ?? 0)
            };
        }
        public async Task<bool> CreateManyAsync(List<Lesson> lessons)
        {
            await _dbContext.Lesson.AddRangeAsync(lessons);
            return await _dbContext.SaveChangesAsync() > 0;
        }




    }
}
