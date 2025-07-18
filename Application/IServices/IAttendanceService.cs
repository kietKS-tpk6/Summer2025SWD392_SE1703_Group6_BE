using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using Domain.Entities;

namespace Application.IServices
{
    public interface IAttendanceService
    {
        Task<OperationResult<string>> SetupAttendaceByClassIdAsync(string classId);
        Task<OperationResult<AttendanceRecordDTO>> GetAttendanceAsync(string classId);
        Task<OperationResult<LessonAttendanceDTO>> GetAttendanceByLessonIdAsync(string lessonId);
        Task<OperationResult<bool>> CheckAttendanceAsync(AttendanceCheckCommand request);
        Task<OperationResult<bool>> HasAllStudentsCheckedAttendanceAsync(string classId);
    }
}
