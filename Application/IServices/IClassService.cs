using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IClassService
    {
        Task<bool> CreateClassAsync(ClassCreateCommand  request);

        Task<List<ClassDetailForPaymentDTO>> GetAllClassesAsync(bool includeInactive = false);
        Task<ClassDetailForPaymentDTO> GetClassDetailAsync(string classId);
        Task<bool> IsClassAvailableForEnrollmentAsync(string classId);
        Task<int> GetAvailableSlotsAsync(string classId);

        Task<List<ClassDetailForPaymentDTO>> GetAllClassesForPaymentAsync();
        Task<ClassDetailForPaymentDTO> GetClassDetailForPaymentAsync(string classId);

    }
}
