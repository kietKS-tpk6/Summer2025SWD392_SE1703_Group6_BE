using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using Infrastructure.IRepositories;
using Domain.Entities;
using Domain.Enums;
using Application.Common.Shared;
using Application.DTOs;
using Application.Common.Constants;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Services
{
    public class AssessmentCriteriaService : IAssessmentCriteriaService
    {
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        public AssessmentCriteriaService(IAssessmentCriteriaRepository assessmentCriteriaRepository)
        {
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }
       
        public async Task<OperationResult<AssessmentCriteriaUpdateDTO>> UpdateAssessmentCriteriaAsync(AssessmentCriteriaUpdateCommand command)
        {
            var result = await _assessmentCriteriaRepository.GetByIdAsync(command.AssessmentCriteriaID);
            if (!result.Success || result.Data == null)
            {
                return OperationResult<AssessmentCriteriaUpdateDTO>.Fail(OperationMessages.NotFound("tiêu chí đánh giá"));
            }

            var existing = result.Data;
            existing.WeightPercent = command.WeightPercent;
            existing.Category = (AssessmentCategory)command.Category;

            if (existing.Category == AssessmentCategory.Presentation || existing.Category == AssessmentCategory.Attendance
                || existing.Category == AssessmentCategory.ClassParticipation
                || existing.Category == AssessmentCategory.Assignment)
            {
                existing.RequiredTestCount = 0;
            }
            else
            {
                existing.RequiredTestCount = command.RequiredTestCount;
            }

            existing.Note = command.Note;
            existing.MinPassingScore = command.MinPassingScore;

            var updateResult = await _assessmentCriteriaRepository.UpdateAsync(existing);

            if (!updateResult.Success)
            {
                return OperationResult<AssessmentCriteriaUpdateDTO>.Fail(updateResult.Message);
            }

            // Map entity to DTO
            var dto = new AssessmentCriteriaUpdateDTO
            {
                Order = 1,
                AssessmentCriteriaID = existing.AssessmentCriteriaID,
                Category = existing.Category?.ToString(),
                RequireCount = existing.RequiredTestCount
            };

            return OperationResult<AssessmentCriteriaUpdateDTO>.Ok(dto, OperationMessages.UpdateSuccess("tiêu chí đánh giá"));
        }

        public async Task<OperationResult<List<AssessmentCriteriaDTO>>> GetListBySubjectIdAsync(string subjectId)
        {
            return await _assessmentCriteriaRepository.GetListBySubjectIdAsync(subjectId);
        }
        public async Task<OperationResult<List<AssessmentCriteriaSetupDTO>>> SetupAssessmentCriteria(AssessmentCriteriaSetupCommand request)
        {
            var existingResult = await _assessmentCriteriaRepository.GetListBySubjectIdAsync(request.SubjectID);
            var existingList = existingResult.Data ?? new List<AssessmentCriteriaDTO>();
            int currentCount = existingList.Count;
            int targetCount = request.NumberAssessmentCriteria;

            if (currentCount == targetCount)
            {
                var resultList = existingList
                    .OrderBy(x => x.AssessmentCriteriaID)
                    .Select((x, index) => new AssessmentCriteriaSetupDTO
                    {
                        numOfAssessment = index + 1,
                        AssessmentCriteriaID = x.AssessmentCriteriaID
                    })
                    .ToList();

                return OperationResult<List<AssessmentCriteriaSetupDTO>>.Ok(
                    resultList,
                    OperationMessages.RetrieveSuccess("tiêu chí đánh giá")
                );
            }

            if (currentCount < targetCount)
            {
                int numberToCreate = targetCount - currentCount;

                var totalInDb = await _assessmentCriteriaRepository.CountAsync();
                var newList = new List<AssessmentCriteria>();

                for (int i = 0; i < numberToCreate; i++)
                {
                    string newId = "AC" + (totalInDb + i + 1).ToString("D4");

                    var newAssCri = new AssessmentCriteria
                    {
                        AssessmentCriteriaID = newId,
                        SubjectID = request.SubjectID,
                        WeightPercent = null,
                        Category = null,
                        RequiredTestCount = null,
                        Note = null,
                        MinPassingScore = null,
                        IsActive = true // Luôn set là true khi tạo mới
                    };

                    newList.Add(newAssCri);
                }

                var createResult = await _assessmentCriteriaRepository.CreateManyAsync(newList);
                var merged = existingList
                    .Concat(createResult.Data.Select(x => new AssessmentCriteriaDTO
                    {
                        AssessmentCriteriaID = x.AssessmentCriteriaID,
                        SubjectID = request.SubjectID,
                        IsActive = true
                    }))
                    .OrderBy(x => x.AssessmentCriteriaID)
                    .Select((x, index) => new AssessmentCriteriaSetupDTO
                    {
                        numOfAssessment = index + 1,
                        AssessmentCriteriaID = x.AssessmentCriteriaID
                    })
                    .ToList();

                return OperationResult<List<AssessmentCriteriaSetupDTO>>.Ok(
                    merged,
                    OperationMessages.CreateSuccess($"{numberToCreate} tiêu chí đánh giá mới")
                );
            }
            else
            {
                int numberToRemove = currentCount - targetCount;

                var toRemoveIds = existingList
                    .OrderByDescending(x => x.AssessmentCriteriaID)
                    .Take(numberToRemove)
                    .Select(x => x.AssessmentCriteriaID)
                    .ToList();

                var deleteResult = await _assessmentCriteriaRepository.SoftDeleteByIdsAsync(toRemoveIds);

                var updatedList = existingList
                    .Where(x => !toRemoveIds.Contains(x.AssessmentCriteriaID))
                    .OrderBy(x => x.AssessmentCriteriaID)
                    .Select((x, index) => new AssessmentCriteriaSetupDTO
                    {
                        numOfAssessment = index + 1,
                        AssessmentCriteriaID = x.AssessmentCriteriaID
                    })
                    .ToList();

                return OperationResult<List<AssessmentCriteriaSetupDTO>>.Ok(
                    updatedList,
                    OperationMessages.DeleteSuccess($"{numberToRemove} tiêu chí đánh giá đã xoá")
                );
            }
        }


        public async Task<OperationResult<AssessmentCriteria>> GetByIdAsync(string assessmentCriteriaId)
        {
            return await _assessmentCriteriaRepository.GetByIdAsync(assessmentCriteriaId);
        }

        public OperationResult<bool> CheckDuplicateCategory(List<AssessmentCriteriaUpdateCommand> items)
        {
            var duplicates = items
                .GroupBy(x => x.Category)
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicates.Any())
            {
                var dups = string.Join(", ", duplicates.Select(g => g.Key.ToString()));
                return OperationResult<bool>.Fail($"Category bị trùng: {dups}");
            }

            return OperationResult<bool>.Ok(true);
        }


        public OperationResult<bool> CheckRequiredTestCountRule(List<AssessmentCriteriaUpdateCommand> items)
        {
            var autoZeroCategories = new[]
            {
        AssessmentCategory.Presentation,
        AssessmentCategory.Attendance,
        AssessmentCategory.Assignment,
        AssessmentCategory.ClassParticipation
    };

            var violations = items
                .Where(x => autoZeroCategories.Contains(x.Category) && x.RequiredTestCount > 0)
                .Select(x => $"ID: {x.AssessmentCriteriaID}, Category: {x.Category}, RequireCount: {x.RequiredTestCount}")
                .ToList();

            if (violations.Any())
            {
                var msg = "Các mục sau có RequiredTestCount > 0 không hợp lệ:\n" + string.Join("\n", violations);
                return OperationResult<bool>.Fail(msg);
            }

            return OperationResult<bool>.Ok(true);
        }

        public async Task<OperationResult<List<AssessmentCriteriaUpdateDTO>>> UpdateAssessmentCriteriaListAsync(List<AssessmentCriteriaUpdateCommand> items)
        {
            var updatedEntities = new List<AssessmentCriteria>();

            foreach (var item in items)
            {
                var entityResult = await _assessmentCriteriaRepository.GetByIdAsync(item.AssessmentCriteriaID);
                if (!entityResult.Success || entityResult.Data == null)
                    return OperationResult<List<AssessmentCriteriaUpdateDTO>>.Fail($"Không tìm thấy ID {item.AssessmentCriteriaID}");

                var existing = entityResult.Data;
                existing.WeightPercent = item.WeightPercent;
                existing.Category = item.Category;
                existing.Note = item.Note;
                existing.MinPassingScore = item.MinPassingScore;
                existing.RequiredTestCount =
                    (item.Category == AssessmentCategory.Presentation ||
                     item.Category == AssessmentCategory.Attendance ||
                     item.Category == AssessmentCategory.ClassParticipation ||
                     item.Category == AssessmentCategory.Assignment)
                    ? 0 : item.RequiredTestCount;

                updatedEntities.Add(existing);
            }

            var updateResult = await _assessmentCriteriaRepository.UpdateRangeAsync(updatedEntities);
            if (!updateResult.Success)
                return OperationResult<List<AssessmentCriteriaUpdateDTO>>.Fail(updateResult.Message);

            var dtos = updatedEntities.Select(x => new AssessmentCriteriaUpdateDTO
            {
                AssessmentCriteriaID = x.AssessmentCriteriaID,
                Category = x.Category.ToString(),
                RequireCount = x.RequiredTestCount,
                Order = 1
            }).ToList();

            return OperationResult<List<AssessmentCriteriaUpdateDTO>>.Ok(dtos);
        }
        //Lỗi nên tạm comment - Kho

        ////KIỆT :HÀM CỦA KIỆT
        //public async Task<Dictionary<(string Category, string TestType), int>> GetRequiredTestCountsAsync(string syllabusId)
        //{
        //    var result = await _assessmentCriteriaRepository.GetListBySubjectIdAsync(syllabusId);

        //    return result
        //        .GroupBy(x => (x.Category.ToString(), x.TestType.ToString()))
        //        .ToDictionary(
        //            g => g.Key,
        //            g => g.Sum(x => x.RequiredCount ?? 0)
        //        );
        //}


        ////kiểm tra bài kiểm tra được thêm vào
        ////có nằm trong danh sách các bài kiểm tra của môn đó không
        ////KIỆT :HÀM CỦA KIỆT

        //public async Task<bool> IsTestDefinedInCriteriaAsync(string syllabusId, TestCategory category, TestType testType)
        //{
        //    var stringCategory = category.ToString();
        //    var stringTestType = testType.ToString();
        //    return await _assessmentCriteriaRepository.IsTestDefinedInCriteriaAsync(syllabusId, stringCategory, stringTestType);
        //}
        //public async Task<OperationResult<List<AssessmentCriteriaDTO>>> GetListBySubjectIdAsync(string subjectId)
        //{
        //    var result = await _assessmentCriteriaRepository.GetListBySubjectIdAsync(subjectId);

        //    if (result == null || !result.Any())
        //    {
        //        return OperationResult<List<AssessmentCriteriaDTO>>.Fail(
        //            OperationMessages.NotFound("tiêu chí đánh giá")
        //        );
        //    }

        //    return OperationResult<List<AssessmentCriteriaDTO>>.Ok(
        //        result,
        //        OperationMessages.RetrieveSuccess("tiêu chí đánh giá")
        //    );
        //}






    }
}
