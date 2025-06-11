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
        Task<int> CountAsync();
        Task<AssessmentCriteria?> GetByIdAsync(string id);
        //Task<bool> CreateAsync(AssessmentCriteria assessmentCriteria);
        //Task<List<AssessmentCriteriaDTO>> GetListBySyllabusIdAsync(string syllabusId);
        //Task<bool> DeleteAsync(string id);
       
        //Task<bool> IsTestDefinedInCriteriaAsync(string syllabusId, string category, string testType);


    }
}
