using System;

namespace Application.DTOs
{
    public class FeedbackDTO
    {
        public string FeedbackID { get; set; }
        public string ClassID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime FeedbackAt { get; set; }
    }

    public class CreateFeedbackDTO
    {
        public string ClassID { get; set; }
        public string StudentID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class FeedbackSummaryDTO
    {
        public string ClassID { get; set; }
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
    }
}