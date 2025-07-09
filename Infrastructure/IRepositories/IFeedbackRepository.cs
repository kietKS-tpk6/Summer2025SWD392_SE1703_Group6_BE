using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.IRepositories
{
    public interface IFeedbackRepository
    {
        Task<string> CreateFeedbackAsync(Feedback feedback);
        Task<Feedback> GetFeedbackByIdAsync(string feedbackId);
        Task<List<Feedback>> GetFeedbacksByClassIdAsync(string classId);
        Task<List<Feedback>> GetFeedbacksByStudentIdAsync(string studentId);
        Task<bool> HasStudentFeedbackForClassAsync(string studentId, string classId);
        Task<int> GetTotalFeedbacksCountAsync();
        Task<double> GetAverageRatingByClassAsync(string classId);
        Task<int> GetFeedbackCountByClassAsync(string classId);
        Task<Dictionary<int, int>> GetRatingDistributionByClassAsync(string classId);
        Task<bool> UpdateFeedbackAsync(Feedback feedback);
        Task<bool> DeleteFeedbackAsync(string feedbackId);
    }
}