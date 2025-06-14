using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Constants;
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
        public async Task<int> GetEnrollmentCountAsync(string classId)
        {
            return await _dbContext.ClassEnrollment
                .CountAsync(sc => sc.ClassID == classId);
        }

        public async Task<OperationResult<List<Class>>> GetAllAsync()
        {
            var result = await _dbContext.Class.ToListAsync();
            return OperationResult<List<Class>>.Ok(result);
        }

        public async Task<OperationResult<Class?>> GetByIdAsync(string id)
        {
            var result = await _dbContext.Class.FindAsync(id);
            return result == null
                ? OperationResult<Class?>.Fail(OperationMessages.NotFound("Lớp học"))
                : OperationResult<Class?>.Ok(result);
        }

        public async Task<OperationResult<bool>> CreateAsync(Class classCreate)
        {
            _dbContext.Class.Add(classCreate);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0
                ? OperationResult<bool>.Ok(true, OperationMessages.CreateSuccess("lớp học"))
                : OperationResult<bool>.Fail(OperationMessages.CreateFail("lớp học"));
        }

        public async Task<OperationResult<bool>> UpdateAsync(Class classUpdate)
        {
            _dbContext.Class.Update(classUpdate);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0
                ? OperationResult<bool>.Ok(true, OperationMessages.UpdateSuccess("lớp học"))
                : OperationResult<bool>.Fail(OperationMessages.UpdateFail("lớp học"));
        }

        public async Task<OperationResult<bool>> DeleteAsync(string id)
        {
            var classToDelete = await _dbContext.Class.FindAsync(id);
            if (classToDelete == null)
                return OperationResult<bool>.Fail(OperationMessages.NotFound("Lớp học"));

            var studentCount = await _dbContext.ClassEnrollment
                .CountAsync(e => e.ClassID == id && e.Status != EnrollmentStatus.Cancelled);

            if (studentCount >= classToDelete.MinStudentAcp)
            {
                return OperationResult<bool>.Fail($"Lớp học đã có {studentCount} học viên, lớn hơn hoặc bằng số tối thiểu {classToDelete.MinStudentAcp}. Không thể hủy.");
            }

            classToDelete.Status = ClassStatus.Deleted;
            _dbContext.Class.Update(classToDelete);
            var result = await _dbContext.SaveChangesAsync();

            return result > 0
                ? OperationResult<bool>.Ok(true, OperationMessages.DeleteSuccess("lớp học"))
                : OperationResult<bool>.Fail(OperationMessages.DeleteFail("lớp học"));
        }


        public async Task<OperationResult<int>> CountAsync()
        {
            var count = await _dbContext.Class.CountAsync();
            return OperationResult<int>.Ok(count);
        }
        private async Task<OperationResult<(List<ClassDTO> Items, int TotalCount)>> GetPaginatedClassListAsync(Expression<Func<Class, bool>> predicate, int page, int pageSize)
        {
            var query = _dbContext.Class
                .Include(c => c.Lecturer)
                .Include(c => c.Subject)
                .Where(predicate);

            var totalCount = await query.CountAsync();

            var pagedClasses = await query
                .OrderByDescending(c => c.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var classIds = pagedClasses.Select(c => c.ClassID).ToList();

            var enrollCounts = await _dbContext.ClassEnrollment
                .Where(e => classIds.Contains(e.ClassID) && e.Status == EnrollmentStatus.Actived)
                .GroupBy(e => e.ClassID)
                .Select(g => new
                {
                    ClassID = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.ClassID, x => x.Count);

            var items = pagedClasses.Select(c => new ClassDTO
            {
                ClassID = c.ClassID,
                LecturerID = c.LecturerID,
                SubjectID = c.SubjectID,
                ClassName = c.ClassName,
                MinStudentAcp = c.MinStudentAcp,
                MaxStudentAcp = c.MaxStudentAcp,
                NumberStudentEnroll = enrollCounts.TryGetValue(c.ClassID, out var count) ? count : 0,
                PriceOfClass = c.PriceOfClass,
                Status = c.Status,
                CreateAt = c.CreateAt,
                TeachingStartTime = c.TeachingStartTime,
                ImageURL = c.ImageURL,
                LecturerName = c.Lecturer?.FirstName,
                SubjectName = c.Subject?.SubjectName
            }).ToList();

            return OperationResult<(List<ClassDTO>, int)>.Ok((items, totalCount));
        }


        public Task<OperationResult<(List<ClassDTO>, int)>> GetPaginatedListAsync(int page, int pageSize)
        {
            return GetPaginatedClassListAsync(c => c.Status != ClassStatus.Deleted, page, pageSize);
        }


        public Task<OperationResult<(List<ClassDTO>, int)>> GetPaginatedListBySubjectAsyn(string subjectId, int page, int pageSize)
        {
            return GetPaginatedClassListAsync(c => c.Status != ClassStatus.Deleted && c.SubjectID == subjectId, page, pageSize);
        }

        public Task<OperationResult<(List<ClassDTO>, int)>> GetPaginatedListByTeacherAsync(string teacherId, int page, int pageSize)
        {
            return GetPaginatedClassListAsync(c => c.Status != ClassStatus.Deleted && c.LecturerID == teacherId, page, pageSize);
        }

        public Task<OperationResult<(List<ClassDTO>, int)>> GetPaginatedListBySubjectAndTeacherAsync(string subjectId, string teacherId, int page, int pageSize)
        {
            return GetPaginatedClassListAsync(c =>
                c.Status != ClassStatus.Deleted &&
                c.SubjectID == subjectId &&
                c.LecturerID == teacherId, page, pageSize);
        }

        public Task<OperationResult<(List<ClassDTO>, int)>> GetPaginatedListByStatusAsync(string status, int page, int pageSize)
        {
            if (!Enum.TryParse<ClassStatus>(status, out var parsedStatus))
            {
                return Task.FromResult(OperationResult<(List<ClassDTO>, int)>
                    .Fail(OperationMessages.InvalidInput("Trạng thái")));
            }

            return GetPaginatedClassListAsync(c => c.Status == parsedStatus, page, pageSize);
        }


        public async Task<OperationResult<List<ClassDTO>>> SearchClassAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return OperationResult<List<ClassDTO>>.Ok(new List<ClassDTO>());

            keyword = keyword.ToLower().Trim();

            var result = await (
                from c in _dbContext.Class
                join acc in _dbContext.Accounts on c.LecturerID equals acc.AccountID
                join subj in _dbContext.Subject on c.SubjectID equals subj.SubjectID
                where c.Status == ClassStatus.Open &&
                      (c.ClassName.ToLower().Contains(keyword) ||
                       acc.FirstName.ToLower().Contains(keyword) ||
                       subj.SubjectName.ToLower().Contains(keyword))
                orderby c.CreateAt descending
                select MapToDTO(c, acc.FirstName, subj.SubjectName)
            ).ToListAsync();

            return OperationResult<List<ClassDTO>>.Ok(result);
        }

        public async Task<ClassCreateLessonDTO?> GetClassCreateLessonDTOByIdAsync(string classId)
        {
            return await _dbContext.Class
                .Where(c => c.ClassID == classId)
                .Select(c => new ClassCreateLessonDTO
                {
                    SubjectId = c.SubjectID,
                    LecturerID = c.LecturerID,
                    StartTime = c.TeachingStartTime
                })
                .FirstOrDefaultAsync();
        }

        public async Task<OperationResult<ClassDTO?>> GetClassDTOByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<ClassDTO?>.Fail(OperationMessages.InvalidInput("ID lớp học"));

            var result = await (
                from c in _dbContext.Class
                join acc in _dbContext.Accounts on c.LecturerID equals acc.AccountID
                join subj in _dbContext.Subject on c.SubjectID equals subj.SubjectID
                where c.ClassID == id
                select MapToDTO(c, acc.FirstName, subj.SubjectName)
            ).FirstOrDefaultAsync();

            return result == null
                ? OperationResult<ClassDTO?>.Fail(OperationMessages.NotFound("Lớp học"))
                : OperationResult<ClassDTO?>.Ok(result);
        }

        private static ClassDTO MapToDTO(Class c, string? lecturerName = null, string? subjectName = null)
        {
            return new ClassDTO
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
                LecturerName = lecturerName ?? c.Lecturer?.FirstName,
                SubjectName = subjectName ?? c.Subject?.SubjectName
            };
        }
    }
}