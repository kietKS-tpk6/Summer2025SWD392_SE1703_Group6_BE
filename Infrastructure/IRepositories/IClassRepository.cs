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
    }
}
