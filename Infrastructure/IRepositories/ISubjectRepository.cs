using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.IRepositories
{
    public interface ISubjectRepository
    {
        Task<string> CreateSubjectAsync(Subject subject);
        Task<Subject?> GetSubjectByIdAsync(string subjectId);
        Task<List<Subject>> GetAllSubjectsAsync(bool? isActive = null);
        Task<string> UpdateSubjectAsync(Subject subject);
        Task<string> DeleteSubjectAsync(string subjectId);
        Task<bool> SubjectExistsAsync(string subjectId);
        Task<int> GetTotalSubjectsCountAsync();

        //KHO
        Task<OperationResult<List<SubjectCreateClassDTO>>> GetSubjectByStatusAsync(SubjectStatus subjectStatus);
    }
}