using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
namespace Infrastructure.IRepositories
{
    public interface ILessonRepository
    {
        Task<bool> CreateAsync(Lesson lesson);
        Task<int> CountAsync();
        Task<bool> UpdateAsync(Lesson lesson);
        Task<bool> DeleteAsync(string id);
        Task<OperationResult<bool>> DeleteLessonByClassIDAsync(string classID);
        Task<List<Lesson>> GetLessonsByClassIDAsync(string classID);
        Task<List<Lesson>> GetLessonsByStudentIDAsync(string studentID);
        Task<List<Lesson>> GetLessonsByLecturerIDAsync(string lecturerID);
        Task<LessonDetailDTO> GetLessonDetailByLessonIDAsync(string classLessonID);
        Task<bool> CreateManyAsync(List<Lesson> lessons);
        Task<Lesson?> GetLessonByClassLessonIDAsync(string classLessonID);
    }
}
