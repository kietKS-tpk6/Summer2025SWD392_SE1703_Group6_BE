using Application.DTOs;
using Application.Usecases.Command;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IEnrollmentService
    {
        Task<string> CreateEnrollmentAsync(CreateEnrollmentCommand command);
        Task<List<ClassDetailForPaymentDTO>> GetMyClassesAsync(string studentId);
        Task<bool> IsStudentEnrolledAsync(string studentId, string classId);
        Task<int> GetClassCurrentEnrollmentsAsync(string classId);
        Task<string> GenerateNextEnrollmentIdAsync();
    }
}
