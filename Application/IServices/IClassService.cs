using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;

namespace Application.IServices
{
    public interface IClassService
    {
        Task<int> GetEnrollmentCountAsync(string classId);
        Task<OperationResult<string?>> CreateClassAsync(ClassCreateCommand request);
        Task<OperationResult<bool>> UpdateClassAsync(ClassUpdateCommand request);
        Task<OperationResult<bool>> UpdateStatusAsync(ClassUpdateStatusCommand request);
        Task<OperationResult<bool>> DeleteClassAsync(string classId);

        Task<OperationResult<PagedResult<ClassDTO>>> GetListAsync(int page, int pageSize);
        Task<OperationResult<PagedResult<ClassDTO>>> GetListBySubjectAsyn(string subjectId, int page, int pageSize);
        Task<OperationResult<PagedResult<ClassDTO>>> GetListByTeacherAsync(string teacherId, int page, int pageSize);
        Task<OperationResult<PagedResult<ClassDTO>>> GetListBySubjectAndTeacherAsync(string subjectId, string teacherId, int page, int pageSize);
        Task<OperationResult<PagedResult<ClassDTO>>> GetListByStatusAsync(string status, int page, int pageSize);
        Task<OperationResult<List<Class>>> GetClassesByStatusAsync(ClassStatus status);
        Task<OperationResult<List<ClassDTO>>> SearchClassAsync(string keyword);
        Task<OperationResult<ClassDTO?>> GetClassDTOByIDAsync(string classId);

        Task<OperationResult<ClassCreateLessonDTO>> GetClassCreateLessonDTOByIdAsync(string classId);
        Task<OperationResult<List<StudentDTO>>> GetStudentsByClassIdAsync(string classId);
        Task<OperationResult<bool>> IsClassNameDuplicateAsync(string className);
        Task<OperationResult<int>> GetOngoingClassCountByLecturerAsync(string lecturerId);
        Task<OperationResult<ClassInfoUpdateDTO>> GetClassInfoForUpdateAsync(string classId);

    }
}
