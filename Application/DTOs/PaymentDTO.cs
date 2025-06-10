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
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("gateway")]
        public string Gateway { get; set; }

        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("transferType")]
        public string TransferType { get; set; }

        [JsonPropertyName("transferAmount")]
        public decimal TransferAmount { get; set; }

        [JsonPropertyName("accumulated")]
        public decimal Accumulated { get; set; }

        [JsonPropertyName("subAccount")]
        public string SubAccount { get; set; }

        [JsonPropertyName("referenceCode")]
        public string ReferenceCode { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class WebhookResponseDTO
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
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
        public decimal PriceOfClass { get; set; }
        public DateTime TeachingStartTime { get; set; }
        public string ImageURL { get; set; }
        public string LecturerName { get; set; }
        public int MaxStudentAcp { get; set; }
        public int CurrentEnrollments { get; set; }
        public bool CanEnroll { get; set; }
    }
}