using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAllAsync();
        Task<Class?> GetAsync(string id);
        Task<bool> CreateAsync(Class classCreate);
        Task<bool> UpdateAsync(Class classUpdate);
        Task<bool> DeleteAsync(string id);
        Task<int> CountAsync();

        Task<string> CreateClassAsync(Class classEntity);
        Task<Class> GetClassByIdAsync(string classId);
        Task<List<Class>> GetAllClassesAsync(bool includeInactive = false);
        Task<List<Class>> GetClassesBySubjectIdAsync(string subjectId);
        Task<string> UpdateClassAsync(Class classEntity);
        Task<bool> ClassExistsAsync(string classId);
        Task<int> GetTotalClassesCountAsync();
        Task<string> GenerateNextClassIdAsync();
    }
}
