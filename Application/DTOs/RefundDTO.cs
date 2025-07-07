using System;
using Domain.Enums;

namespace Application.DTOs
{
    public class RefundRequestDTO
    {
        public string PaymentID { get; set; }
        public string StudentID { get; set; }
        public string Reason { get; set; }
    }

    public class RefundResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string PaymentID { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }

    public class RefundListItemDTO
    {
        public string PaymentID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime EnrolledDate { get; set; }
        public string Reason { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public class RefundEligibilityDTO
    {
        public bool IsEligible { get; set; }
        public string Message { get; set; }
        public int DaysRemaining { get; set; }
        public DateTime EnrolledDate { get; set; }
        public DateTime DeadlineDate { get; set; }
    }
}