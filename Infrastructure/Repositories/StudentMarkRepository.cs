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
    public class StudentMarkRepository : IStudentMarkRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public StudentMarkRepository(HangulLearningSystemDbContext context)
        {
            _dbContext = context;
        }

        public async Task<StudentMark> GetByStudentTestIdAsync(string studentTestId)
        {
            return await _dbContext.StudentMarks
                .FirstOrDefaultAsync(sm => sm.StudentTestID == studentTestId);
        }
    }
}
