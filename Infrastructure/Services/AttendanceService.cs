using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Infrastructure.IRepositories;
using Infrastructure.Repositories;

namespace Infrastructure.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IClassService _classService;
        private readonly ILessonService _lessonService;
        public AttendanceService(IAttendanceRepository attendanceRepository, IClassService classService, ILessonService lessonService)
        {
            _attendanceRepository = attendanceRepository;
            _classService = classService;
            _lessonService = lessonService;
        }
        public async Task<OperationResult<string>> SetupAttendaceByClassIdAsync(string classId)
        {
            var classFound = await _classService.GetClassDTOByIDAsync(classId);
            if(!classFound.Success || classFound.Data == null)
            {
                return OperationResult<string>.Fail(OperationMessages.NotFound("lớp học"));
            }
            var students = await _classService.GetStudentsByClassIdAsync(classId);
            if(!students.Success || students.Data == null)
            {
                return OperationResult<string>.Fail(OperationMessages.NotFound("học viên"));
            }
            var lessons = await _lessonService.GetLessonsByClassID(classId);
            if(!lessons.Success || lessons.Data == null)
            {
                return OperationResult<string>.Fail(OperationMessages.NotFound("tiết học"));
            }
            return await _attendanceRepository.SetupAttendaceByClassIdAsync(classId, students.Data, lessons.Data);
        }
        public async Task<OperationResult<AttendanceRecordDTO>> GetAttendanceAsync(string classId)
        {
            var classExists = await _classService.GetClassDTOByIDAsync(classId);
            if (!classExists.Success || classExists.Data == null)
            {
                return OperationResult<AttendanceRecordDTO>.Fail(OperationMessages.NotFound("lớp học"));
            }

            return  await _attendanceRepository.GetAttendanceAsync(classId);
        }
        public async Task<OperationResult<LessonAttendanceDTO>> GetAttendanceByLessonIdAsync(string lessonId)
        {
            var lessonExist = await _lessonService.GetLessonDetailByLessonIDAsync(lessonId);
            if (!lessonExist.Success || lessonExist == null)
            {
                return OperationResult<LessonAttendanceDTO>.Fail(OperationMessages.NotFound("tiết học"));
            }
            return await _attendanceRepository.GetAttendanceByLessonIdAsync(lessonId);
        }
        public async Task<OperationResult<bool>> CheckAttendanceAsync(AttendanceCheckCommand request)
        {
            return await _attendanceRepository.CheckAttendanceAsync(request);
        }
        public async Task<OperationResult<bool>> HasAllStudentsCheckedAttendanceAsync(string classId)
        {
            return await _attendanceRepository.HasAllStudentsCheckedAttendanceAsync(classId);
        }
    }
}
