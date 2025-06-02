using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public ClassRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Class>> GetAllAsync()
        {
            return await _dbContext.Class.ToListAsync();
        }

        public async Task<Class?> GetAsync(string id)
        {
            return await _dbContext.Class.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Class classCreate)
        {
            _dbContext.Class.Add(classCreate);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Class classUpdate)
        {
            _dbContext.Class.Update(classUpdate);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var classToDelete = await _dbContext.Class.FindAsync(id);
            if (classToDelete == null)
            {
                return false;
            }

            _dbContext.Class.Remove(classToDelete);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Class.CountAsync();
        }
    }

}
