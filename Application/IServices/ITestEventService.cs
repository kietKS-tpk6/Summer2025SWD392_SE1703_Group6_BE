using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface ITestEventService
    {
        //Hàm của Kho - làm create class 
        Task<OperationResult<bool>> SetupTestEventsByClassIDAsync(string classID);
        //Hàm của Kho - Xóa Class
        Task<OperationResult<bool>> DeleteTestEventsByClassIDAsync(string classID);

        Task<OperationResult<bool>> UpdateTestEventAsync(UpdateTestEventCommand request);

        //Hàm của kiệt  - lấy bài kiểm tra cho test
        Task<OperationResult<TestAssignmentDTO>> GetTestAssignmentForStudentAsync(string testEventID);
        
       //kit {Lấy danh sách tất cả TestEvent theo ClassID, thông qua ClassLessonID}
        Task<OperationResult<List<TestByClassDTO>>> GetTestsByClassIDAsync(string classID);

    }
}
