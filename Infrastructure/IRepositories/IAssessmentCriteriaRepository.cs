using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.IRepositories
{
    public interface IAssessmentCriteriaRepository
    {
        Task<List<AssessmentCriteria>> GetAllAsync();
        Task<List<AssessmentCriteriaDTO>> GetListBySyllabusIdAsync(string syllabusId);
        Task<(List<AssessmentCriteriaDTO> Items, int TotalCount)> GetPaginatedListAsync(int page, int pageSize);
        Task<AssessmentCriteria?> GetByIdAsync(string id);
        Task<bool> CreateAsync(AssessmentCriteria assessmentCriteria);
        Task<bool> UpdateAsync(AssessmentCriteria assessmentCriteria);
        Task<bool> DeleteAsync(string id);
        Task<int> CountAsync();

    }
}
