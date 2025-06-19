using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.IRepositories
{
    public interface IEnrollmentRepository
    {
        Task<string> CreateEnrollmentAsync(ClassEnrollment enrollment);
        Task<ClassEnrollment> GetEnrollmentByIdAsync(string enrollmentId);
        Task<List<ClassEnrollment>> GetEnrollmentsByStudentIdAsync(string studentId);
        Task<List<ClassEnrollment>> GetEnrollmentsByClassIdAsync(string classId);
        Task<bool> IsStudentEnrolledAsync(string studentId, string classId);
        Task<int> GetClassCurrentEnrollmentsAsync(string classId);
        Task<int> GetTotalEnrollmentsCountAsync();
        Task<List<Lesson>> GetLessonsByStudentIdAsync(string studentId);
        Task<List<Lesson>> GetLessonsByClassIdAsync(string classId);
    }
}