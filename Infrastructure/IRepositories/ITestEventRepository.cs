using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface ITestEventRepository
    {
        Task<OperationResult<bool>> CreateTestEventForCreateClassAsync(TestEvent testEvent);
        Task<OperationResult<int>> CountTestEventAsync();
        Task<OperationResult<bool>> DeleteTestEventsByClassIDAsync(string classId);
    }
}
