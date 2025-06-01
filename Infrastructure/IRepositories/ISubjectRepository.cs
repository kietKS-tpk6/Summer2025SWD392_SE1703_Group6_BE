using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ISubjectService
    {
        Task<List<SubjectDTO>> GetAllSubjectsAsync(bool? isActive = null);
        Task<SubjectDTO> GetSubjectByIdAsync(string subjectId);
        Task<bool> SubjectExistsAsync(string subjectId);
        Task<int> GetTotalSubjectsCountAsync();
    }
}