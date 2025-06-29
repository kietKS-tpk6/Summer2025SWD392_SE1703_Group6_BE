using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IStudentTestRepository
    {
        Task<StudentTest> GetByIdAsync(string studentTestID);
        Task<bool> ExistsAsync(string studentTestID);
        Task<OperationResult<bool>> UpdateAsync(StudentTest test);
        Task<List<StudentTest>> GetByTestEventIDsAsync(List<string> testEventIDs);

    }
}
