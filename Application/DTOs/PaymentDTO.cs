using System;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Application.DTOs
{
    public class CreatePaymentRequestDTO
    {
        public string AccountID { get; set; } 
        public string ClassID { get; set; }
        public string Description { get; set; }
    }

    public class PaymentResponseDTO
    {
        public string PaymentID { get; set; }
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public decimal Total { get; set; }
        public string QRCodeUrl { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime DayCreate { get; set; }
        public string Description { get; set; }
    }

    public class PaymentStatusDTO
    {
        public string PaymentID { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Total { get; set; }
        public DateTime? PaidDate { get; set; }
    }

    public class TransactionDTO
    {
        public int Id { get; set; }

       
        public string Gateway { get; set; }

        public string TransactionDate { get; set; }

        public string AccountNumber { get; set; }

        public string ?Code { get; set; }

        public string Content { get; set; }

        public string TransferType { get; set; }

        public decimal TransferAmount { get; set; }
        
        public decimal Accumulated { get; set; }

        public string SubAccount { get; set; }

        public string ReferenceCode { get; set; }

        public string Description { get; set; }
    }

    public class WebhookResponseDTO
    {
  
        public bool Success { get; set; }

        public string Message { get; set; }
    }
    public class PaymentSettings
    {
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string WebhookEndpoint { get; set; }
        public string ApiKey { get; set; }
        public string SubAccount { get; set; }
    }

    public class ClassDetailForPaymentDTO
    {
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string ImageURL { get; set; }
        public string LecturerName { get; set; }
       
    }
    public class PaymentListItemDTO
    {
        public string PaymentID { get; set; }
        public string AccountID { get; set; }
        public string StudentName { get; set; }
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public decimal Total { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime DayCreate { get; set; }
        public string Description { get; set; }
        public int? TransactionID { get; set; }
    }
}