using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
namespace Application.IServices
{
    public interface IAssessmentCriteriaService
    {
        //public Task<bool> CreateAssessmentCriteriaAsync(AssessmentCriteriaCreateCommand request);
        Task<OperationResult<bool>> UpdateAssessmentCriteriaAsync(AssessmentCriteriaUpdateCommand command);
        //public Task<PagedResult<AssessmentCriteriaDTO>> GetPaginatedListAsync(int page, int pageSize);
        public Task<OperationResult<List<AssessmentCriteriaDTO>>> GetListBySubjectIdAsync(string subjectId);
        //public Task<bool> DeleteAsync(string id);
        //Task<Dictionary<(string Category, string TestType), int>> GetRequiredTestCountsAsync(string syllabusId);
        //Task<bool> IsTestDefinedInCriteriaAsync(string syllabusId, TestCategory category, TestType testType);
        public Task<OperationResult<int>> SetupAssessmentCriteria(AssessmentCriteriaSetupCommand request);
    }
}
