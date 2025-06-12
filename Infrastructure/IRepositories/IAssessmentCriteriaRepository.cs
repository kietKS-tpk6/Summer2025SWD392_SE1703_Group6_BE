using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.IRepositories
{
    public interface IAssessmentCriteriaRepository
    {
        Task<OperationResult<int>> CreateManyAsync(List<AssessmentCriteria> list);
        Task<OperationResult<bool>> UpdateAsync(AssessmentCriteria assessmentCriteria);

        Task<List<AssessmentCriteriaDTO>> GetListBySubjectIdAsync(string subjectID);

        Task<List<AssessmentCriteria>> GetAllAsync();
        //Task<List<AssessmentCriteriaDTO>> GetListBySyllabusIdAsync(string syllabusId);
       // Task<(List<AssessmentCriteriaDTO> Items, int TotalCount)> GetPaginatedListAsync(int page, int pageSize);
        Task<int> CountAsync();
        Task<AssessmentCriteria?> GetByIdAsync(string id);
        //Task<bool> CreateAsync(AssessmentCriteria assessmentCriteria);
        //Task<List<AssessmentCriteriaDTO>> GetListBySyllabusIdAsync(string syllabusId);
        //Task<bool> DeleteAsync(string id);
       
        //Task<bool> IsTestDefinedInCriteriaAsync(string syllabusId, string category, string testType);

        Task<bool> DeleteAsync(string id);
        //KIỆT :HÀM CỦA KIỆT
        Task<bool> IsTestDefinedInCriteriaAsync(string subjectID, string category, string testType);

    }
}
