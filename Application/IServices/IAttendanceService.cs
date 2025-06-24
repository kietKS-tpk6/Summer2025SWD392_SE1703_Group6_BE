using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;

namespace Application.IServices
{
    public interface IAttendanceService
    {
        Task<OperationResult<string>> SetupAttendaceByClassIdAsync(string classId);
        Task<OperationResult<AttendanceRecordDTO>> GetAttendanceAsync(string classId);
        Task<OperationResult<LessonAttendanceDTO>> GetAttendanceByLessonIdAsync(string lessonId);
    }
}
