using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface ILessonService
    {
        Task<bool> CreateLessonAsync(LessonCreateCommand request);
        Task<bool> UpdateLessonAsync(LessonUpdateCommand request);
        Task<bool> DeleteLessonAsync(string id);
        Task<List<LessonDTO>> GetLessonsByClassID(string classID);
        Task<List<LessonDTO>> GetLessonsByStudentID(string studentID);
        Task<List<LessonDTO>> GetLessonsByLecturerID(string lecturerID);
        Task<LessonDetailDTO> GetLessonDetailByLessonIDAsync(string classLessonID);
        Task<bool> CreateLessonsFromSchedulesAsync(
            string classId,
            string lecturerId,
            TimeOnly startHour,
            List<DayOfWeek> selectedDays,
            List<SyllabusScheduleCreateLessonDTO> schedules
        );

    }
}
