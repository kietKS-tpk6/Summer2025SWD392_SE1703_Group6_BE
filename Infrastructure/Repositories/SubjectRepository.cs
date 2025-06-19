using Infrastructure.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public SubjectRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateSubjectAsync(Subject subject)
        {
            _dbContext.Subject.Add(subject);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Subject?> GetSubjectByIdAsync(string subjectId)
        {
            return await _dbContext.Subject
                .FirstOrDefaultAsync(s => s.SubjectID == subjectId);
        }

        public async Task<List<Subject>> GetAllSubjectsAsync(SubjectStatus? status = null)
        {
            var query = _dbContext.Subject.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return await query
                .OrderBy(s => s.SubjectID)
                .ToListAsync();
        }

        public async Task<string> UpdateSubjectAsync(Subject subject)
        {
            try
            {
                _dbContext.Subject.Update(subject);
                await _dbContext.SaveChangesAsync();
                return "Subject updated successfully";
            }
            catch (Exception ex)
            {
                return $"Error updating subject: {ex.Message}";
            }
        }

        public async Task<string> DeleteSubjectAsync(string subjectId)
        {
            try
            {
                var subject = await GetSubjectByIdAsync(subjectId);
                if (subject != null)
                {
                    subject.Status = SubjectStatus.Deleted; // Thay đổi status thay vì IsActive
                    _dbContext.Subject.Update(subject);
                    await _dbContext.SaveChangesAsync();
                    return "Subject deleted successfully";
                }
                return "Subject not found";
            }
            catch (Exception ex)
            {
                return $"Error deleting subject: {ex.Message}";
            }
        }

        public async Task<bool> SubjectExistsAsync(string subjectId)
        {
            return await _dbContext.Subject
                .AnyAsync(s => s.SubjectID == subjectId && s.Status != SubjectStatus.Deleted);
        }

        public async Task<int> GetTotalSubjectsCountAsync()
        {
            return await _dbContext.Subject
                .CountAsync(s => s.Status != SubjectStatus.Deleted);
        }

        // Kiểm tra xem subject có đầy đủ schedule không
        public async Task<bool> HasCompleteScheduleAsync(string subjectId)
        {
            // Kiểm tra xem có bản ghi nào với SubjectID này và không có field nào null
            var schedules = await _dbContext.SyllabusSchedule
                .Where(s => s.SubjectID == subjectId && s.IsActive == true)
                .ToListAsync();

            if (!schedules.Any())
                return false;

            // Kiểm tra các field quan trọng không được null hoặc empty
            foreach (var schedule in schedules)
            {
                // Kiểm tra theo cấu trúc thực tế của SyllabusSchedule
                if (string.IsNullOrEmpty(schedule.LessonTitle) ||
                    string.IsNullOrEmpty(schedule.Content) ||
                    !schedule.DurationMinutes.HasValue ||
                    schedule.DurationMinutes <= 0 ||
                    !schedule.Week.HasValue ||
                    schedule.Week <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        // Kiểm tra xem subject có đầy đủ assessment criteria không
        public async Task<bool> HasCompleteAssessmentCriteriaAsync(string subjectId)
        {
            // Kiểm tra bảng AssessmentCriteria
            var criteria = await _dbContext.AssessmentCriteria // Cập nhật tên table thực tế
                .Where(a => a.SyllabusID == subjectId) // Thay đổi theo FK thực tế
                .ToListAsync();

            if (!criteria.Any())
                return false;

            // Kiểm tra các field quan trọng không được null hoặc có giá trị hợp lệ
            foreach (var criterion in criteria)
            {
                // Kiểm tra theo cấu trúc thực tế của AssessmentCriteria
                if (criterion.WeightPercent <= 0 ||
                    criterion.RequiredCount < 0 ||
                    criterion.Duration <= 0 ||
                    criterion.MinPassingScore < 0 ||
                    string.IsNullOrEmpty(criterion.Category) ||
                    string.IsNullOrEmpty(criterion.TestType))
                {
                    return false;
                }
            }

            return true;
        }

        // Lấy danh sách các field bị thiếu
        public async Task<List<string>> GetMissingFieldsAsync(string subjectId)
        {
            var missingFields = new List<string>();

            // Kiểm tra Schedule
            if (!await HasCompleteScheduleAsync(subjectId))
            {
                missingFields.Add("Schedule");
            }

            // Kiểm tra Assessment Criteria
            if (!await HasCompleteAssessmentCriteriaAsync(subjectId))
            {
                missingFields.Add("AssessmentCriteria");
            }

            return missingFields;
        }
        public async Task<bool> ExistsByIdAsync(string subjectName)
        {
            return await _dbContext.Subject.AnyAsync(s => s.SubjectName == subjectName);
        }

        public async Task<bool> ExistsByDescriptionAsync(string description)
        {
            return await _dbContext.Subject.AnyAsync(s => s.Description == description);
        }
        public async Task<OperationResult<List<SubjectCreateClassDTO>>> GetSubjectByStatusAsync(SubjectStatus subjectStatus)
        {
            try
            {
                var subjects = await _dbContext.Subject
                    .Where(s => s.Status == subjectStatus)
                    .Select(s => new SubjectCreateClassDTO
                    {
                        SubjectID = s.SubjectID,
                        SubjectName = s.SubjectName,
                        Description = s.Description,
                        IsActive = s.Status == SubjectStatus.Active, // Chuyển đổi từ Status
                        CreateAt = s.CreateAt,
                        MinAverageScoreToPass = s.MinAverageScoreToPass,
                        Status = s.Status
                    })
                    .ToListAsync();

                return OperationResult<List<SubjectCreateClassDTO>>.Ok(
                    subjects,
                    OperationMessages.RetrieveSuccess("môn học")
                );
            }
            catch (Exception ex)
            {
                return OperationResult<List<SubjectCreateClassDTO>>.Fail($"Lỗi khi truy xuất môn học: {ex.Message}");
            }
        }
    }
}