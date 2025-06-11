using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.IRepositories
{
    public interface IAssessmentCriteriaRepository
    {
        Task<string> CreateManyAsync(List<AssessmentCriteria> list, int numbers);
        Task<string> UpdateAsync(AssessmentCriteria assessmentCriteria);

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
