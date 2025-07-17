using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
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

            var criteriaGroupCounts = studentMarks
                .GroupBy(sm => sm.AssessmentCriteriaID)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.AssessmentIndex).Distinct().Count()
                );

            var result = studentMarks
                .GroupBy(sm => new { sm.AssessmentCriteriaID, sm.AssessmentCriteria.Category, sm.AssessmentIndex })
                .Select(g => new StudentMarkDetailKhoDTO
                {
                    AssessmentCategory = (AssessmentCategory)g.Key.Category,
                    AssessmentIndex = g.Key.AssessmentIndex,
                    WeightPercent = g.FirstOrDefault()?.AssessmentCriteria?.WeightPercent / criteriaGroupCounts[g.Key.AssessmentCriteriaID],
                    StudentMarks = g.Select(sm => new StudentMarkItem
                    {
                        StudentMarkID = sm.StudentMarkID,
                        StudentName = sm.Account?.FirstName + " " + sm.Account?.LastName,
                        Mark = sm.Mark,
                        Comment = sm.Comment,
                        StudentTestID = sm.StudentTestID,
                    }).ToList()
                })
                .ToList();

            return OperationResult<List<StudentMarkDetailKhoDTO>>.Ok(result, OperationMessages.RetrieveSuccess("bảng điểm"));
        }
        public async Task<OperationResult<StudentMarkForStudentDTO>> GetStudentMarkForStudent(GetStudentMarkForStudentCommand request)
        {
            var studentMarks = await _dbContext.StudentMarks
                .Where(sm => sm.AccountID == request.StudentId && sm.ClassID == request.ClassId)
                .Include(sm => sm.AssessmentCriteria)
                .Include(sm => sm.StudentTest)
                .ToListAsync();
            var details = studentMarks.Select(sm => new MarkComponentDTO
            {
                StudentMarkID = sm.StudentMarkID,
                AssessmentCategory = sm.AssessmentCriteria?.Category,
                AssessmentIndex = sm.AssessmentIndex,
                WeightPercent = sm.AssessmentCriteria?.WeightPercent,
                Mark = sm.Mark,
                Comment = sm.Comment,
                StudentTestID = sm.StudentTestID
            }).ToList();

            bool allValid = details.All(d => d.Mark.HasValue && d.WeightPercent.HasValue);

            decimal? GPA = null;
            if (allValid && details.Any())
            {
                decimal total = details.Sum(d => (decimal)d.Mark.Value * (decimal)(d.WeightPercent.Value / 100.0));
                GPA = Math.Round(total, 2);
            }

            return OperationResult<StudentMarkForStudentDTO>.Ok(new StudentMarkForStudentDTO
            {
                GPA = GPA,
                StudentMarkDetails = details
            });
        }
        public async Task<OperationResult<bool>> UpdateStudentMarksAsync(UpdateStudentMarksCommand request)
        {
            var markIds = request.InputMarks.Select(m => m.StudentMarkID).ToList();

            var studentMarks = await _dbContext.StudentMarks
                .Where(sm => markIds.Contains(sm.StudentMarkID))
                .ToListAsync();

            if (!studentMarks.Any())
            {
                return OperationResult<bool>.Fail("Không tìm thấy bản ghi điểm nào để cập nhật.");
            }

            foreach (var input in request.InputMarks)
            {
                var mark = studentMarks.FirstOrDefault(sm => sm.StudentMarkID == input.StudentMarkID);
                {
                    mark.Mark = input.Mark;
                    mark.Comment = input.Comment;
                    mark.UpdatedAt = DateTime.UtcNow;
                    mark.GradedBy = request.LecturerId;
                    mark.GradedAt = DateTime.UtcNow;

                }
                if(mark.CreatedAt == null)
                {
                    mark.CreatedAt = DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync();
            return OperationResult<bool>.Ok(true, OperationMessages.UpdateSuccess("điểm"));
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
        public async Task<OperationResult<bool>> HasAllStudentsGradedAsync(string classId)
        {
            var hasUnmarked = await _dbContext.StudentMarks
                .AnyAsync(m => m.ClassID == classId && m.Mark == null);

            if (hasUnmarked)
            {
                return OperationResult<bool>.Fail("Vẫn còn học viên chưa được chấm điểm.");
            }

            return OperationResult<bool>.Ok(true, "Tất cả học viên đã được chấm điểm.");
        }


    }
}