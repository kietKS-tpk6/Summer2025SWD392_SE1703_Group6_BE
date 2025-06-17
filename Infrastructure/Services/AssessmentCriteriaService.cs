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
namespace Infrastructure.Services
{
    public class AssessmentCriteriaService : IAssessmentCriteriaService
    {
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        public AssessmentCriteriaService(IAssessmentCriteriaRepository assessmentCriteriaRepository)
        {
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }

        public async Task<OperationResult<AssessmentCriteriaUpdateDto>> UpdateAssessmentCriteriaAsync(AssessmentCriteriaUpdateCommand command)
        {
            // Biến để điều khiển validation: 1 = chỉ check IsActive = 1, 0 = check cả IsActive = 0 và 1
            const int checkOnlyActive = 1;

            var result = await _assessmentCriteriaRepository.GetByIdAsync(command.AssessmentCriteriaID);
            if (!result.Success || result.Data == null)
            {
                return OperationResult<AssessmentCriteriaUpdateDto>.Fail(OperationMessages.NotFound("tiêu chí đánh giá"));
            }

            var existing = result.Data;

            var duplicateCategory = await _assessmentCriteriaRepository.CheckDuplicateCategoryInSubjectAsync(
                existing.SubjectID,
                (AssessmentCategory)command.Category,
                command.AssessmentCriteriaID,
                checkOnlyActive);

            if (duplicateCategory.Success && duplicateCategory.Data)
            {
                return OperationResult<AssessmentCriteriaUpdateDto>.Fail($"Category '{(AssessmentCategory)command.Category}' đã tồn tại trong Subject này");
            }

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
                return OperationResult<AssessmentCriteriaUpdateDto>.Fail(updateResult.Message);
            }

            // Map entity to DTO
            var dto = new AssessmentCriteriaUpdateDto
            {
                Order = 1,
                AssessmentCriteriaID = existing.AssessmentCriteriaID,
                Category = existing.Category?.ToString(),
                RequireCount = existing.RequiredTestCount
            };

            return OperationResult<AssessmentCriteriaUpdateDto>.Ok(dto, OperationMessages.UpdateSuccess("tiêu chí đánh giá"));
        }

            var updateResult = await _assessmentCriteriaRepository.UpdateAsync(existing);

            if (!updateResult.Success)
            {
                return OperationResult<AssessmentCriteriaUpdateDto>.Fail(updateResult.Message);
            }

            var dto = new AssessmentCriteriaUpdateDto
            {
                Order = 1, 
                AssessmentCriteriaID = existing.AssessmentCriteriaID,
                Category = existing.Category.ToString(),
                RequireCount = existing.RequiredTestCount
            };

            return OperationResult<AssessmentCriteriaUpdateDto>.Ok(dto, OperationMessages.UpdateSuccess("tiêu chí đánh giá"));
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
                        IsActive = true 
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
