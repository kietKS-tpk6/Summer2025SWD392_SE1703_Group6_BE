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
namespace Infrastructure.Services
{
    public class AssessmentCriteriaService : IAssessmentCriteriaService
    {
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        public AssessmentCriteriaService(IAssessmentCriteriaRepository assessmentCriteriaRepository) {
                _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }
        public async Task<bool> CreateAssessmentCriteriaAsync(AssessmentCriteriaCreateCommand assessmentCriteriaCreateCommand) {
            var numberOfAssCri = await _assessmentCriteriaRepository.CountAsync();
            string newAssessmentCriteriaID = "AC" + numberOfAssCri.ToString("D4");
            var newAssCri = new AssessmentCriteria();
            newAssCri.AssessmentCriteriaID = newAssessmentCriteriaID;
            newAssCri.SyllabusID = assessmentCriteriaCreateCommand.SyllabusID;
            newAssCri.WeightPercent = assessmentCriteriaCreateCommand.WeightPercent;
            newAssCri.Category = (AssessmentCategory)assessmentCriteriaCreateCommand.Category;
            newAssCri.RequiredCount = assessmentCriteriaCreateCommand.RequiredCount;
            newAssCri.Duration = assessmentCriteriaCreateCommand.Duration;
            newAssCri.TestType = (TestType)assessmentCriteriaCreateCommand.TestType;
            newAssCri.Note = assessmentCriteriaCreateCommand.Note;
            newAssCri.MinPassingScore = assessmentCriteriaCreateCommand.MinPassingScore;
            newAssCri.IsActive = true;
            return  await _assessmentCriteriaRepository.CreateAsync(newAssCri);
        }
        public async Task<bool> UpdateAssessmentCriteriaAsync(AssessmentCriteriaUpdateCommand command)
        {
            var existing = await _assessmentCriteriaRepository.GetByIdAsync(command.AssessmentCriteriaID);
            if (existing == null)
            {
                return false; 
            }
            existing.SyllabusID = command.SyllabusID;
            existing.WeightPercent = command.WeightPercent;
            existing.Category = (AssessmentCategory)command.Category;
            existing.RequiredCount = command.RequiredCount;
            existing.Duration = command.Duration;
            existing.TestType = (TestType)command.TestType;
            existing.Note = command.Note;
            existing.MinPassingScore = command.MinPassingScore;

            return await _assessmentCriteriaRepository.UpdateAsync(existing);
        }
        public async Task<PagedResult<AssessmentCriteriaDTO>> GetPaginatedListAsync(int page, int pageSize)
        {
            var (items, total) = await _assessmentCriteriaRepository.GetPaginatedListAsync(page, pageSize);
            return new PagedResult<AssessmentCriteriaDTO>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<List<AssessmentCriteriaDTO>> GetListBySyllabusIdAsync(string syllabusId)
        {
            return await _assessmentCriteriaRepository.GetListBySyllabusIdAsync(syllabusId);
        }
        public async Task<bool> DeleteAsync(string id)
        {
            return await _assessmentCriteriaRepository.DeleteAsync(id);
        }


        public async Task<Dictionary<(string Category, string TestType), int>> GetRequiredTestCountsAsync(string syllabusId)
        {
            var result = await _assessmentCriteriaRepository.GetListBySyllabusIdAsync(syllabusId);

            return result
                .GroupBy(x => (x.Category.ToString(), x.TestType.ToString()))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.RequiredCount)
                );
        }
        //kiểm tra bài kiểm tra được thêm vào
        //có nằm trong danh sách các bài kiểm tra của môn đó không

        public async Task<bool> IsTestDefinedInCriteriaAsync(string syllabusId,TestCategory category, TestType testType)
        {
            var stringCategory = category.ToString();
            var stringTestType = testType.ToString();

            return await _assessmentCriteriaRepository.IsTestDefinedInCriteriaAsync( syllabusId, stringCategory, stringTestType);
            
        }
    }
}
 