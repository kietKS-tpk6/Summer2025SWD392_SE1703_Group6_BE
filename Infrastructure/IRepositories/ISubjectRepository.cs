using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.IRepositories
{
    public interface ISubjectRepository
    {
       
        Task CreateSubjectAsync(Subject subject);
        Task<Subject?> GetSubjectByIdAsync(string subjectId);
        Task<List<Subject>> GetAllSubjectsAsync(SubjectStatus? status = null);
        Task<string> UpdateSubjectAsync(Subject subject);
        Task<string> DeleteSubjectAsync(string subjectId);
        Task<bool> SubjectExistsAsync(string subjectId);
        Task<int> GetTotalSubjectsCountAsync();
        //kiệt
        Task<bool> ExistsByIdAsync(string subjectId);
        //kiệt
        Task<bool> ExistsByDescriptionAsync(string description);


        Task<bool> HasCompleteScheduleAsync(string subjectId);
        Task<bool> HasCompleteAssessmentCriteriaAsync(string subjectId);
        Task<List<string>> GetMissingFieldsAsync(string subjectId);

        //KHO
        Task<OperationResult<List<SubjectCreateClassDTO>>> GetSubjectByStatusAsync(SubjectStatus subjectStatus);
    }
}