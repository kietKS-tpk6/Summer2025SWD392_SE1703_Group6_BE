using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Enums;
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

        Task<string> CreateSubjectAsync(CreateSubjectCommand command);
        Task<string> UpdateSubjectAsync(UpdateSubjectCommand command);
        Task<string> DeleteSubjectAsync(string subjectId);
        Task<string> GenerateNextSubjectIdAsync();

        //Khoa làm
        Task<OperationResult<List<SubjectCreateClassDTO>>> GetSubjectByStatusAsync(SubjectStatus subjectStatus);
    }
}