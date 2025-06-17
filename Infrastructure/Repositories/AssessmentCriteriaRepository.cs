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
        public async Task<OperationResult<List<AssessmentCriteriaDTO>>> GetListBySubjectIdAsync(string subjectID)
        {
            var items = await _dbContext.AssessmentCriteria
                .Where(x => x.SubjectID == subjectID && x.IsActive)
                .Select(x => new AssessmentCriteriaDTO
                {
                    AssessmentCriteriaID = x.AssessmentCriteriaID,
                    SubjectID = x.SubjectID,
                    WeightPercent = x.WeightPercent,
                    Category = x.Category,
                    RequiredTestCount = x.RequiredTestCount,
                    Note = x.Note,
                    IsActive = x.IsActive,
                    MinPassingScore = x.MinPassingScore
                })
                .ToListAsync();
            var message = items.Count == 0
                ? OperationMessages.NotFound("tiêu chí đánh giá")
                : OperationMessages.RetrieveSuccess("tiêu chí đánh giá");
            return OperationResult<List<AssessmentCriteriaDTO>>.Ok(items, message);
        }

        public async Task<OperationResult<List<AssessmentCriteriaSetupDTO>>> CreateManyAsync(List<AssessmentCriteria> entities)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _dbContext.AssessmentCriteria.AddRangeAsync(entities);
                var saved = await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                var resultList = entities.Select((x, index) => new AssessmentCriteriaSetupDTO
                {
                    AssessmentCriteriaID = x.AssessmentCriteriaID,
                    Stt = index + 1  // Đổi từ numOfAssessment thành Stt
                }).ToList();
                var message = OperationMessages.CreateSuccess($"{saved} tiêu chí đánh giá");
                return OperationResult<List<AssessmentCriteriaSetupDTO>>.Ok(resultList, message);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                var message = OperationMessages.CreateFail("tiêu chí đánh giá");
                return OperationResult<List<AssessmentCriteriaSetupDTO>>.Fail(message);
            }
        }
        public async Task<List<AssessmentCriteria>> GetAllAsync()
        {
            return await _dbContext.AssessmentCriteria.ToListAsync();
        }
        public async Task<OperationResult<AssessmentCriteria>> UpdateAsync(AssessmentCriteria assessmentCriteria)
        {
            _dbContext.AssessmentCriteria.Update(assessmentCriteria);
            var result = await _dbContext.SaveChangesAsync();

            if (result > 0)
            {
                return OperationResult<AssessmentCriteria>.Ok(assessmentCriteria, OperationMessages.UpdateSuccess("tiêu chí đánh giá"));
            }
            else
            {
                return OperationResult<AssessmentCriteria>.Fail(OperationMessages.UpdateFail("tiêu chí đánh giá"));
            }
        }
        public async Task<OperationResult<bool>> CheckDuplicateCategoryInSubjectAsync(string subjectId, AssessmentCategory category, string excludeAssessmentCriteriaId, int checkOnlyActive)
        {
            try
            {
                IQueryable<AssessmentCriteria> query = _dbContext.AssessmentCriteria
                    .Where(ac => ac.SubjectID == subjectId
                              && ac.Category == category
                              && ac.AssessmentCriteriaID != excludeAssessmentCriteriaId);

                // Nếu checkOnlyActive = 1 thì chỉ check với IsActive = true
                // Nếu checkOnlyActive = 0 thì check cả IsActive = false và true
                if (checkOnlyActive == 1)
                {
                    query = query.Where(ac => ac.IsActive == true);
                }

                var exists = await query.AnyAsync();

                return OperationResult<bool>.Ok(exists);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail($"Lỗi khi kiểm tra duplicate category: {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> SoftDeleteAsync(string id)
        {
            var entity = await _dbContext.AssessmentCriteria.FindAsync(id);
            if (entity == null)
            {
                var notFoundMsg = OperationMessages.NotFound("tiêu chí đánh giá");
                return OperationResult<bool>.Fail(notFoundMsg);
            }
            entity.IsActive = false;
            _dbContext.AssessmentCriteria.Update(entity);
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                var successMsg = OperationMessages.DeleteSuccess("tiêu chí đánh giá");
                return OperationResult<bool>.Ok(true, successMsg);
            }
            else
            {
                var failMsg = OperationMessages.DeleteFail("tiêu chí đánh giá");
                return OperationResult<bool>.Fail(failMsg);
            }
        }
        public async Task<OperationResult<bool>> SoftDeleteByIdsAsync(List<string> ids)
        {
            var entities = await _dbContext.AssessmentCriteria
                .Where(x => ids.Contains(x.AssessmentCriteriaID) && x.IsActive)
                .ToListAsync();

            if (entities == null || !entities.Any())
            {
                var notFoundMsg = OperationMessages.NotFound("tiêu chí đánh giá cần xoá");
                return OperationResult<bool>.Fail(notFoundMsg);
            }
            foreach (var entity in entities)
            {
                entity.IsActive = false;
            }
            _dbContext.AssessmentCriteria.UpdateRange(entities);
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                var successMsg = OperationMessages.DeleteSuccess($"{entities.Count} tiêu chí đánh giá");
                return OperationResult<bool>.Ok(true, successMsg);
            }
            else
            {
                var failMsg = OperationMessages.DeleteFail("tiêu chí đánh giá");
                return OperationResult<bool>.Fail(failMsg);
            }
        }


        public async Task<int> CountAsync()
        {
            return await _dbContext.AssessmentCriteria.CountAsync();
        }


        public async Task<OperationResult<AssessmentCriteria>> GetByIdAsync(string assessmentCriteriaId)
        {
            var entity = await _dbContext.AssessmentCriteria
                                         .FirstOrDefaultAsync(x => x.AssessmentCriteriaID == assessmentCriteriaId);

            if (entity == null)
            {
                var message = OperationMessages.NotFound("tiêu chí đánh giá");
                return OperationResult<AssessmentCriteria>.Fail(message);
            }

            return OperationResult<AssessmentCriteria>.Ok(entity, OperationMessages.RetrieveSuccess("tiêu chí đánh giá"));
        }



        //KIỆT :HÀM CỦA KIỆT
        //public async Task<bool> IsTestDefinedInCriteriaAsync(string subjectID, string category, string testType)
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
        //        .AnyAsync(ac => ac.SubjectID == subjectID
        //                     && ac.Category == categoryEnum
        //                     && ac.TestType == testTypeEnum
        //                     && ac.IsActive);
        //}


    }
}
