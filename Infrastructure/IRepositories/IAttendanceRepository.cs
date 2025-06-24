using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;

namespace Infrastructure.IRepositories
{
    public interface IAttendanceRepository
    {
        Task<OperationResult<string>> SetupAttendaceByClassIdAsync(string classId, List<StudentDTO> students, List<LessonDTO> lessons);
        Task<AttendanceRecordDTO> GetAttendanceAsync(string classId);
        Task<OperationResult<LessonAttendanceDTO>> GetAttendanceByLessonIdAsync(string lessonId);
    }
}
