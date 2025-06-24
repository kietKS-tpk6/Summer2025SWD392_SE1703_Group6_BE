using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Enums;

namespace Infrastructure.IRepositories
{
    public interface IClassRepository
    {
        Task<int> GetEnrollmentCountAsync(string classId);

        Task<OperationResult<List<Class>>> GetAllAsync();
        Task<OperationResult<Class?>> GetByIdAsync(string id);
        Task<ClassCreateLessonDTO> GetClassCreateLessonDTOByIdAsync(string id);

        Task<OperationResult<string?>> CreateAsync(Class classCreate);
        Task<OperationResult<bool>> UpdateAsync(Class classUpdate);
        Task<OperationResult<bool>> UpdateStatusAsync(ClassUpdateStatusCommand request);
        Task<OperationResult<bool>> DeleteAsync(string id);

        Task<OperationResult<int>> CountAsync();

        Task<OperationResult<(List<ClassDTO> Items, int TotalCount)>> GetPaginatedListAsync(int page, int pageSize);
        Task<OperationResult<(List<ClassDTO> Items, int TotalCount)>> GetPaginatedListBySubjectAsyn(string subjectId, int page, int pageSize);
        Task<OperationResult<(List<ClassDTO> Items, int TotalCount)>> GetPaginatedListByTeacherAsync(string teacherId, int page, int pageSize);
        Task<OperationResult<(List<ClassDTO> Items, int TotalCount)>> GetPaginatedListBySubjectAndTeacherAsync(string subjectId, string teacherId, int page, int pageSize);
        Task<OperationResult<(List<ClassDTO> Items, int TotalCount)>> GetPaginatedListByStatusAsync(string status, int page, int pageSize);

        Task<OperationResult<List<Class>>> GetClassesByStatusAsync(ClassStatus status);

        Task<OperationResult<List<ClassDTO>>> SearchClassAsync(string keyword);
        Task<OperationResult<ClassDTO?>> GetClassDTOByIdAsync(string id);
        Task<OperationResult<List<StudentDTO>>> GetStudentsByClassIdAsync(string classId);
        Task<OperationResult<bool>> IsClassNameDuplicateAsync(string className);
    }
}
