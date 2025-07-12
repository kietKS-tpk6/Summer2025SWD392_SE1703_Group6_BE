using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public FeedbackRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateFeedbackAsync(Feedback feedback)
        {
            await _dbContext.Feedback.AddAsync(feedback);
            await _dbContext.SaveChangesAsync();
            return feedback.FeedbackID;
        }

        public async Task<Feedback> GetFeedbackByIdAsync(string feedbackId)
        {
            return await _dbContext.Feedback
                .Include(f => f.Student)
                .Include(f => f.Class)
                .FirstOrDefaultAsync(f => f.FeedbackID == feedbackId);
        }

        public async Task<List<Feedback>> GetFeedbacksByClassIdAsync(string classId)
        {
            return await _dbContext.Feedback
                .Include(f => f.Student)
                .Where(f => f.ClassID == classId)
                .OrderByDescending(f => f.FeedbackAt)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksByStudentIdAsync(string studentId)
        {
            return await _dbContext.Feedback
                .Include(f => f.Class)
                .Include(f => f.Student)
                .Where(f => f.StudentID == studentId)
                .OrderByDescending(f => f.FeedbackAt)
                .ToListAsync();
        }

        public async Task<bool> HasStudentFeedbackForClassAsync(string studentId, string classId)
        {
            return await _dbContext.Feedback
                .AnyAsync(f => f.StudentID == studentId && f.ClassID == classId);
        }

        public async Task<int> GetTotalFeedbacksCountAsync()
        {
            return await _dbContext.Feedback.CountAsync();
        }

        public async Task<double> GetAverageRatingByClassAsync(string classId)
        {
            var feedbacks = await _dbContext.Feedback
                .Where(f => f.ClassID == classId)
                .Select(f => f.Rating)
                .ToListAsync();

            return feedbacks.Count > 0 ? feedbacks.Average() : 0;
        }

        public async Task<int> GetFeedbackCountByClassAsync(string classId)
        {
            return await _dbContext.Feedback
                .CountAsync(f => f.ClassID == classId);
        }

        public async Task<Dictionary<int, int>> GetRatingDistributionByClassAsync(string classId)
        {
            var ratings = await _dbContext.Feedback
                .Where(f => f.ClassID == classId)
                .GroupBy(f => f.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync();

            var distribution = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
            {
                distribution[i] = ratings.FirstOrDefault(r => r.Rating == i)?.Count ?? 0;
            }

            return distribution;
        }

        public async Task<bool> UpdateFeedbackAsync(Feedback feedback)
        {
            _dbContext.Feedback.Update(feedback);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteFeedbackAsync(string feedbackId)
        {
            var feedback = await _dbContext.Feedback.FindAsync(feedbackId);
            if (feedback == null) return false;

            _dbContext.Feedback.Remove(feedback);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}