using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAllAsync();
        Task<Class?> GetByIdAsync(string id);
       // Task<ClassCreateLessonDTO> GetClassCreateLessonDTOByIdAsync(string id);
        Task<bool> CreateAsync(Class classCreate);
        Task<bool> UpdateAsync(Class classUpdate);
        Task<bool> DeleteAsync(string id);
        Task<int> CountAsync();
        Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListAsync(int page, int pageSize);
        Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListBySubjectAsyn(string subjectId, int page, int pageSize);
        Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListByTeacherAsync(string teacherId, int page, int pageSize);
        Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListBySubjectAndTeacherAsync(string subjectId, string teacherId, int page, int pageSize);
        Task<(List<ClassDTO> Items, int TotalCount)> GetPaginatedListByStatusAsync(string status, int page, int pageSize);
        Task<List<ClassDTO>> SearchClassAsync(string keyword);
        Task<ClassDTO?> GetClassDTOByIdAsync(string id);


    }
}
