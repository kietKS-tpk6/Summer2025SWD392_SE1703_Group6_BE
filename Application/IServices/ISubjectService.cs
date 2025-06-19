using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
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

        Task<OperationResult<string>> CreateSubjectAsync(Subject subject);
        Task<string> UpdateSubjectAsync(UpdateSubjectCommand command);
        Task<string> UpdateSubjectStatusAsync(UpdateSubjectStatusCommand command);
        Task<string> DeleteSubjectAsync(string subjectId);
        Task<string> GenerateNextSubjectIdAsync();
        //kiệt
        Task<bool> SubjectNameExistsAsync(string subjectName);

        //kiệt
        Task<bool> DescriptionExistsAsync(string description);
        

        Task<SubjectStatusCheckResult> CheckSubjectStatusAsync(string subjectId);
        Task<string> TryActivateSubjectAsync(string subjectId);

        //Khoa làm
        Task<OperationResult<List<SubjectCreateClassDTO>>> GetSubjectByStatusAsync(SubjectStatus subjectStatus);
    }
}