using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
namespace Application.IServices
{
    public interface IAssessmentCriteriaService
    {
        public Task<bool> CreateAssessmentCriteriaAsync(AssessmentCriteriaCreateCommand request);
        public Task<bool> UpdateAssessmentCriteriaAsync(AssessmentCriteriaUpdateCommand request);
        public Task<PagedResult<AssessmentCriteriaDTO>> GetPaginatedListAsync(int page, int pageSize);
        public Task<List<AssessmentCriteriaDTO>> GetListBySyllabusIdAsync(string syllabusId);
        public Task<bool> DeleteAsync(string id);
    }
}
