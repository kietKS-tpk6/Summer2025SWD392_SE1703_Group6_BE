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
    public interface ILessonService
    {
        Task<OperationResult<bool>> CreateLessonAsync(LessonCreateCommand request);
        Task<OperationResult<bool>> UpdateLessonAsync(LessonUpdateCommand request);
        Task<OperationResult<bool>> DeleteLessonAsync(string id);
        Task<OperationResult<bool>> DeleteLessonByClassIDAsync (string classID);

        Task<OperationResult<List<LessonDTO>>> GetLessonsByClassID(string classID);
        Task<OperationResult<List<LessonDTO>>> GetLessonsByStudentID(string studentID);
        Task<OperationResult<List<LessonDTO>>> GetLessonsByLecturerID(string lecturerID);

        Task<OperationResult<LessonDetailDTO>> GetLessonDetailByLessonIDAsync(string classLessonID);

        Task<OperationResult<bool>> CreateLessonsFromSchedulesAsync
        (
            string classId,
            string lecturerId,
            TimeOnly startHour,
            List<DayOfWeek> selectedDays,
            List<SyllabusScheduleCreateLessonDTO> schedules,
            DateTime startTime
        );
        Task<OperationResult<TimeOnly>> GetLessonTimeByClassIdAsync(string classId);

        Task<OperationResult<List<LessonContentDTO>>> GetLessonContentByClassIdAsyn(string classId);
        Task<OperationResult<List<int>>> GetDateOfWeekByClassIdAsync(string classId);

        //kit {Lấy danh sách ClassLesson theo ClassID}
        Task<OperationResult<List<Lesson>>> GetLessonsByClassIDAsync(string classID);

    }
}
