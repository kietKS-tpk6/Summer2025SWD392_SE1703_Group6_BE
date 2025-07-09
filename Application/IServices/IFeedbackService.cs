using Application.DTOs;
using Application.Usecases.Command;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IFeedbackService
    {
        Task<string> CreateFeedbackAsync(CreateFeedbackCommand command);
        Task<List<FeedbackDTO>> GetFeedbacksByClassAsync(string classId);
        Task<List<FeedbackDTO>> GetFeedbacksByStudentAsync(string studentId);
        Task<FeedbackDTO> GetFeedbackByIdAsync(string feedbackId);
        Task<FeedbackSummaryDTO> GetFeedbackSummaryByClassAsync(string classId);
        Task<bool> HasStudentFeedbackForClassAsync(string studentId, string classId);
        Task<string> GenerateNextFeedbackIdAsync();
        Task<bool> DeleteFeedbackAsync(string feedbackId);
        Task<bool> UpdateFeedbackAsync(string feedbackId, int rating, string comment);
    }
}