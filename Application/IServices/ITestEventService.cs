using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;

namespace Application.IServices
{
    public interface ITestEventService
    {
        //Hàm của Kho - làm create class 
        Task<OperationResult<bool>> SetupTestEventsByClassIDAsync(string classID);
        //Hàm của Kho - Xóa Class
        Task<OperationResult<bool>> DeleteTestEventsByClassIDAsync(string classID);
    }
}
