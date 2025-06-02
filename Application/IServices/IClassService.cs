using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Shared;
using Application.DTOs;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IClassService
    {
        Task<bool> CreateClassAsync(ClassCreateCommand  request);
        Task<bool> UpdateClassAsync(ClassUpdateCommand request);
        Task<bool> DeleteClassAsync(string classId);
        Task<PagedResult<ClassDTO>> GetListAsync(int page, int pageSize);
        Task<PagedResult<ClassDTO>> GetListBySubjectAsyn(string subjectId, int page, int pageSize);
        Task<PagedResult<ClassDTO>> GetListByTeacherAsync(string teacherId, int page, int pageSize);
        Task<PagedResult<ClassDTO>> GetListBySubjectAndTeacherAsync(string subjectId, string teacherId, int page, int pageSize);
        Task<PagedResult<ClassDTO>> GetListByStatusAsync(string status, int page, int pageSize);
        Task<List<ClassDTO>> SearchClassAsync(string keyword);

    }
}
