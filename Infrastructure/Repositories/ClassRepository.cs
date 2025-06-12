using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public ClassRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Class>> GetAllAsync()
        {
            return await _dbContext.Class.ToListAsync();
        }

        public async Task<Class?> GetByIdAsync(string id)
        {
            return await _dbContext.Class.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Class classCreate)
        {
            _dbContext.Class.Add(classCreate);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Class classUpdate)
        {
            _dbContext.Class.Update(classUpdate);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var classToDelete = await _dbContext.Class.FindAsync(id);
            if (classToDelete == null)
            {
                return false;
            }

            classToDelete.Status = ClassStatus.Deleted;
            _dbContext.Class.Update(classToDelete);

            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }


        public async Task<int> CountAsync()
        {
            return await _dbContext.Class.CountAsync();
        }
        public async Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListAsync(int page, int pageSize)
        {
            var query = _dbContext.Class
                .Include(c => c.Lecturer)
                .Include(c => c.Subject)
                .Where(c => c.Status != ClassStatus.Deleted)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClassDTO
                {
                    ClassID = c.ClassID,
                    LecturerID = c.LecturerID,
                    SubjectID = c.SubjectID,
                    ClassName = c.ClassName,
                    MinStudentAcp = c.MinStudentAcp,
                    MaxStudentAcp = c.MaxStudentAcp,
                    PriceOfClass = c.PriceOfClass,
                    Status = c.Status,
                    CreateAt = c.CreateAt,
                    TeachingStartTime = c.TeachingStartTime,
                    ImageURL = c.ImageURL,
                    LecturerName = c.Lecturer.FirstName,
                    SubjectName = c.Subject.SubjectName
                })
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListBySubjectAsyn(string subjectId, int page, int pageSize)
        {
            var query = _dbContext.Class
                .Include(c => c.Lecturer)
                .Include(c => c.Subject)
                .Where(c => c.Status != ClassStatus.Deleted && c.SubjectID == subjectId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClassDTO
                {
                    ClassID = c.ClassID,
                    LecturerID = c.LecturerID,
                    SubjectID = c.SubjectID,
                    ClassName = c.ClassName,
                    MinStudentAcp = c.MinStudentAcp,
                    MaxStudentAcp = c.MaxStudentAcp,
                    PriceOfClass = c.PriceOfClass,
                    Status = c.Status,
                    CreateAt = c.CreateAt,
                    TeachingStartTime = c.TeachingStartTime,
                    ImageURL = c.ImageURL,
                    LecturerName = c.Lecturer.FirstName,
                    SubjectName = c.Subject.SubjectName
                })
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListByTeacherAsync(string teacherId, int page, int pageSize)
        {
            var query = _dbContext.Class
                .Include(c => c.Lecturer)
                .Include(c => c.Subject)
                .Where(c => c.Status != ClassStatus.Deleted && c.LecturerID == teacherId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClassDTO
                {
                    ClassID = c.ClassID,
                    LecturerID = c.LecturerID,
                    SubjectID = c.SubjectID,
                    ClassName = c.ClassName,
                    MinStudentAcp = c.MinStudentAcp,
                    MaxStudentAcp = c.MaxStudentAcp,
                    PriceOfClass = c.PriceOfClass,
                    Status = c.Status,
                    CreateAt = c.CreateAt,
                    TeachingStartTime = c.TeachingStartTime,
                    ImageURL = c.ImageURL,
                    LecturerName = c.Lecturer.FirstName,
                    SubjectName = c.Subject.SubjectName
                })
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListBySubjectAndTeacherAsync(string subjectId, string teacherId, int page, int pageSize)
        {
            var query = _dbContext.Class
                .Include(c => c.Lecturer)
                .Include(c => c.Subject)
                .Where(c => c.Status != ClassStatus.Deleted && c.SubjectID == subjectId && c.LecturerID == teacherId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClassDTO
                {
                    ClassID = c.ClassID,
                    LecturerID = c.LecturerID,
                    SubjectID = c.SubjectID,
                    ClassName = c.ClassName,
                    MinStudentAcp = c.MinStudentAcp,
                    MaxStudentAcp = c.MaxStudentAcp,
                    PriceOfClass = c.PriceOfClass,
                    Status = c.Status,
                    CreateAt = c.CreateAt,
                    TeachingStartTime = c.TeachingStartTime,
                    ImageURL = c.ImageURL,
                    LecturerName = c.Lecturer.FirstName,
                    SubjectName = c.Subject.SubjectName
                })
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListByStatusAsync(string status, int page, int pageSize)
        {
            if (!Enum.TryParse<ClassStatus>(status, out var parsedStatus))
            {
                return (new List<ClassDTO>(), 0); 
            }

            var query = _dbContext.Class
                .Include(c => c.Lecturer)
                .Include(c => c.Subject)
                .Where(c => c.Status == parsedStatus);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClassDTO
                {
                    ClassID = c.ClassID,
                    LecturerID = c.LecturerID,
                    SubjectID = c.SubjectID,
                    ClassName = c.ClassName,
                    MinStudentAcp = c.MinStudentAcp,
                    MaxStudentAcp = c.MaxStudentAcp,
                    PriceOfClass = c.PriceOfClass,
                    Status = c.Status,
                    CreateAt = c.CreateAt,
                    TeachingStartTime = c.TeachingStartTime,
                    ImageURL = c.ImageURL,
                    LecturerName = c.Lecturer.FirstName,
                    SubjectName = c.Subject.SubjectName
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<ClassDTO>> SearchClassAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new List<ClassDTO>();

            keyword = keyword.ToLower().Trim();

            var result = await (
                from c in _dbContext.Class
                join acc in _dbContext.Accounts on c.LecturerID equals acc.AccountID
                join subj in _dbContext.Subject on c.SubjectID equals subj.SubjectID
                where c.Status == ClassStatus.Open &&
                      (
                          c.ClassName.ToLower().Contains(keyword) ||
                          acc.FirstName.ToLower().Contains(keyword) ||
                          subj.SubjectName.ToLower().Contains(keyword)
                      )
                orderby c.CreateAt descending
                select new ClassDTO
                {
                    ClassID = c.ClassID,
                    LecturerID = c.LecturerID,
                    SubjectID = c.SubjectID,
                    ClassName = c.ClassName,
                    MinStudentAcp = c.MinStudentAcp,
                    MaxStudentAcp = c.MaxStudentAcp,
                    PriceOfClass = c.PriceOfClass,
                    Status = c.Status,
                    CreateAt = c.CreateAt,
                    TeachingStartTime = c.TeachingStartTime,
                    ImageURL = c.ImageURL,
                    LecturerName = acc.FirstName,
                    SubjectName = subj.SubjectName
                }
            ).ToListAsync();

            return result;
        }
        //public async Task<ClassCreateLessonDTO?> GetClassCreateLessonDTOByIdAsync(string classId)
        //{
        //    var result = await (from c in _dbContext.Class
        //                        join s in _dbContext.Syllabus
        //                          on c.SubjectID equals s.SubjectID
        //                        where c.ClassID == classId && s.Status == SyllabusStatus.Published
        //                        select new ClassCreateLessonDTO
        //                        {
        //                            SubjectId = c.SubjectID,
        //                            LecturerID = c.LecturerID,
        //                            SyllabusID = s.SyllabusID,
        //                            StartTime = c.TeachingStartTime
        //                        }).FirstOrDefaultAsync();

        //    return result;
        //}

        public async Task<ClassDTO?> GetClassDTOByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            var result = await (
                from c in _dbContext.Class
                join acc in _dbContext.Accounts on c.LecturerID equals acc.AccountID
                join subj in _dbContext.Subject on c.SubjectID equals subj.SubjectID
                where c.ClassID == id
                select new ClassDTO
                {
                    ClassID = c.ClassID,
                    LecturerID = c.LecturerID,
                    SubjectID = c.SubjectID,
                    ClassName = c.ClassName,
                    MinStudentAcp = c.MinStudentAcp,
                    MaxStudentAcp = c.MaxStudentAcp,
                    PriceOfClass = c.PriceOfClass,
                    Status = c.Status,
                    CreateAt = c.CreateAt,
                    TeachingStartTime = c.TeachingStartTime,
                    ImageURL = c.ImageURL,
                    LecturerName = acc.FirstName,
                    SubjectName = subj.SubjectName
                }
            ).FirstOrDefaultAsync();

            return result;
        }

    }

}
