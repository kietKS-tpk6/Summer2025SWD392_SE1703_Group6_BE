using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class AssessmentCriteriaRepository : IAssessmentCriteriaRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public AssessmentCriteriaRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> CreateManyAsync(List<AssessmentCriteria> entities, int numbers)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _dbContext.AssessmentCriteria.AddRangeAsync(entities);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"Đã tạo thành công {numbers} tiêu chí đánh giá.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return "Đã xảy ra lỗi trong quá trình tạo tiêu chí đánh giá. Vui lòng liên hệ quản trị viên nếu lỗi vẫn tiếp diễn.";
            }
        }

        public async Task<string> UpdateAsync(AssessmentCriteria assessmentCriteria)
        {
            _dbContext.AssessmentCriteria.Update(assessmentCriteria);
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0) return OperationMessages.UpdateSuccess;
            else return OperationMessages.UpdateFail;
        }
        public async Task<List<AssessmentCriteriaDTO>> GetListBySubjectIdAsync(string subjectID)
        {
            var items = await _dbContext.AssessmentCriteria
                .Where(x => x.SubjectID == subjectID && x.IsActive)
                .Select(x => new AssessmentCriteriaDTO
                {
                    AssessmentCriteriaID = x.AssessmentCriteriaID,
                    SubjectID = x.SubjectID,
                    WeightPercent = x.WeightPercent,
                    Category = x.Category,
                    RequiredCount = x.RequiredCount,
                    Duration = x.Duration,
                    TestType = x.TestType,
                    Note = x.Note,
                    IsActive = x.IsActive,
                    MinPassingScore = x.MinPassingScore
                })
                .ToListAsync();

            return items;
        }

    

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _dbContext.AssessmentCriteria.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            entity.IsActive = false;
            _dbContext.AssessmentCriteria.Update(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<int> CountAsync()
        {
            return await _dbContext.AssessmentCriteria.CountAsync();
        }

        public async Task<List<AssessmentCriteria>> GetAllAsync()
        {
            return await _dbContext.AssessmentCriteria.ToListAsync();
        }
        public async Task<AssessmentCriteria?> GetByIdAsync(string id)
        {
            return await _dbContext.AssessmentCriteria.FindAsync(id);
        }


        //public async Task<(List<AssessmentCriteriaDTO> Items, int TotalCount)> GetPaginatedListAsync(int page, int pageSize)
        //{
        //    var query = _dbContext.AssessmentCriteria.AsQueryable();
        //    var totalCount = await query.CountAsync();

        //    var items = await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(x => new AssessmentCriteriaDTO
        //        {
        //            AssessmentCriteriaID = x.AssessmentCriteriaID,
        //            SyllabusID = x.SyllabusID,
        //            WeightPercent = x.WeightPercent,
        //            Category = x.Category,
        //            RequiredCount = x.RequiredCount,
        //            Duration = x.Duration,
        //            TestType = x.TestType,
        //            Note = x.Note,
        //            IsActive = x.IsActive,
        //            MinPassingScore = x.MinPassingScore
        //        })
        //        .ToListAsync();

        //    return (items, totalCount);
        //}



        //public async Task<bool> CreateAsync(AssessmentCriteria assessmentCriteria)
        //{
        //    _dbContext.AssessmentCriteria.Add(assessmentCriteria);
        //    var result = await _dbContext.SaveChangesAsync();
        //    return result > 0;
        //}


        //public async Task<Dictionary<string, int>> GetAssessmentCountByCategoryAsync(string syllabusId)
        //{
        //    var result = await _dbContext.AssessmentCriteria
        //        .Where(x => x.SyllabusID == syllabusId && x.IsActive)
        //        .GroupBy(x => x.Category)
        //        .Select(g => new
        //        {
        //            Category = g.Key.ToString(),
        //            Count = g.Count()
        //        })
        //        .ToDictionaryAsync(x => x.Category, x => x.Count);

        //    return result;
        //}

        //public async Task<bool> IsTestDefinedInCriteriaAsync(string syllabusId, string category, string testType)
        //{
        //    if (!Enum.TryParse<AssessmentCategory>(category, true, out var categoryEnum))
        //    {
        //        return false; // Category không hợp lệ
        //    }

        //    if (!Enum.TryParse<TestType>(testType, true, out var testTypeEnum))
        //    {
        //        return false; // TestType không hợp lệ
        //    }

        //    return await _dbContext.AssessmentCriteria
        //        .AnyAsync(ac => ac.SyllabusID == syllabusId
        //                     && ac.Category == categoryEnum
        //                     && ac.TestType == testTypeEnum
        //                     && ac.IsActive);
        //}

    }
}
