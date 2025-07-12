using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Infrastructure.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IEnrollmentService _enrollmentService;

        public FeedbackService(IFeedbackRepository feedbackRepository, IEnrollmentService enrollmentService)
        {
            _feedbackRepository = feedbackRepository;
            _enrollmentService = enrollmentService;
        }

        public async Task<string> CreateFeedbackAsync(CreateFeedbackCommand command)
        {
            // Check if student is enrolled in the class
            var isEnrolled = await _enrollmentService.IsStudentEnrolledAsync(command.StudentID, command.ClassID);
            if (!isEnrolled)
            {
                throw new UnauthorizedAccessException("Student is not enrolled in this class");
            }

            // Check if student has already provided feedback for this class
            var hasExistingFeedback = await _feedbackRepository.HasStudentFeedbackForClassAsync(command.StudentID, command.ClassID);
            if (hasExistingFeedback)
            {
                throw new InvalidOperationException("Student has already provided feedback for this class");
            }

            var feedbackId = await GenerateNextFeedbackIdAsync();
            var feedback = new Feedback
            {
                FeedbackID = feedbackId,
                ClassID = command.ClassID,
                StudentID = command.StudentID,
                Rating = command.Rating,
                Comment = command.Comment ?? string.Empty,
                FeedbackAt = DateTime.UtcNow
            };

            return await _feedbackRepository.CreateFeedbackAsync(feedback);
        }

        public async Task<List<FeedbackDTO>> GetFeedbacksByClassAsync(string classId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByClassIdAsync(classId);
            return feedbacks.Select(f => new FeedbackDTO
            {
                FeedbackID = f.FeedbackID,
                ClassID = f.ClassID,
                StudentID = f.StudentID,
                StudentName = f.Student?.Fullname ?? "Unknown",
                Rating = f.Rating,
                Comment = f.Comment,
                FeedbackAt = f.FeedbackAt
            }).ToList();
        }

        public async Task<List<FeedbackDTO>> GetFeedbacksByStudentAsync(string studentId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByStudentIdAsync(studentId);
            return feedbacks.Select(f => new FeedbackDTO
            {
                FeedbackID = f.FeedbackID,
                ClassID = f.ClassID,
                StudentID = f.StudentID,
                StudentName = f.Student?.Fullname ?? "Unknown",
                Rating = f.Rating,
                Comment = f.Comment,
                FeedbackAt = f.FeedbackAt
            }).ToList();
        }

        public async Task<FeedbackDTO> GetFeedbackByIdAsync(string feedbackId)
        {
            var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
            if (feedback == null) return null;

            return new FeedbackDTO
            {
                FeedbackID = feedback.FeedbackID,
                ClassID = feedback.ClassID,
                StudentID = feedback.StudentID,
                StudentName = feedback.Student?.Fullname ?? "Unknown",
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                FeedbackAt = feedback.FeedbackAt
            };
        }

        public async Task<FeedbackSummaryDTO> GetFeedbackSummaryByClassAsync(string classId)
        {
            var averageRating = await _feedbackRepository.GetAverageRatingByClassAsync(classId);
            var totalFeedbacks = await _feedbackRepository.GetFeedbackCountByClassAsync(classId);
            var ratingDistribution = await _feedbackRepository.GetRatingDistributionByClassAsync(classId);

            return new FeedbackSummaryDTO
            {
                ClassID = classId,
                AverageRating = Math.Round(averageRating, 1),
                TotalFeedbacks = totalFeedbacks,
                FiveStarCount = ratingDistribution[5],
                FourStarCount = ratingDistribution[4],
                ThreeStarCount = ratingDistribution[3],
                TwoStarCount = ratingDistribution[2],
                OneStarCount = ratingDistribution[1]
            };
        }

        public async Task<bool> HasStudentFeedbackForClassAsync(string studentId, string classId)
        {
            return await _feedbackRepository.HasStudentFeedbackForClassAsync(studentId, classId);
        }

        public async Task<string> GenerateNextFeedbackIdAsync()
        {
            var count = await _feedbackRepository.GetTotalFeedbacksCountAsync();
            return $"FB{(count + 1):D4}";
        }

        public async Task<bool> DeleteFeedbackAsync(string feedbackId)
        {
            return await _feedbackRepository.DeleteFeedbackAsync(feedbackId);
        }

        public async Task<bool> UpdateFeedbackAsync(string feedbackId, int rating, string comment)
        {
            var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
            if (feedback == null) return false;

            feedback.Rating = rating;
            feedback.Comment = comment ?? string.Empty;
            feedback.FeedbackAt = DateTime.UtcNow;

            return await _feedbackRepository.UpdateFeedbackAsync(feedback);
        }
    }
}