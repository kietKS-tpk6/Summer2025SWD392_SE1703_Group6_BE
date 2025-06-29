using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Usecases.Command;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface ITestEventRepository
    {
        Task<OperationResult<bool>> CreateTestEventForCreateClassAsync(TestEvent testEvent);
        Task<OperationResult<int>> CountTestEventAsync();
        Task<OperationResult<bool>> DeleteTestEventsByClassIDAsync(string classId);
        Task<OperationResult<bool>> UpdateTestEventAsync(TestEvent testEvent);
        Task<TestEvent?> GetByIdAsync(string testEventID);
        //kit {Lấy tất cả TestEvent theo danh sách ClassLessonID}
        Task<List<TestEvent>> GetByClassLessonIDsAsync(List<string> classLessonIDs);
    }
}
