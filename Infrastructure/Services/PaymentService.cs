using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IClassRepository _classRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ITransactionRepository transactionRepository,
            IClassRepository classRepository,
            IConfiguration configuration,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _transactionRepository = transactionRepository;
            _classRepository = classRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateNextPaymentIdAsync()
        {
            var numberOfPayments = await _paymentRepository.GetTotalPaymentsCountAsync();
            return $"PM{(numberOfPayments + 1):D4}";
        }

        public async Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentCommand command)
        {
            try
            {
                _logger.LogInformation($"Creating payment for ClassID: {command.ClassID}, AccountID: {command.AccountID}");

                if (string.IsNullOrWhiteSpace(command.ClassID))
                    throw new ArgumentException("ClassID is required");

                if (string.IsNullOrWhiteSpace(command.AccountID))
                    throw new ArgumentException("AccountID is required");

                // Kiểm tra AccountID và ClassID có tồn tại không
                var classEntity = await _classRepository.GetClassByIdAsync(command.ClassID);
                if (classEntity == null)
                    throw new ArgumentException("Class not found");

                if (classEntity.Status != ClassStatus.Open)
                    throw new ArgumentException("Class is not available for enrollment");

                // Tạo PaymentID duy nhất
                string paymentId;
                int retry = 0;
                do
                {
                    paymentId = await GenerateNextPaymentIdAsync();
                    if (!await _paymentRepository.PaymentExistsAsync(paymentId))
                        break;
                    retry++;
                } while (retry < 5);

                if (retry == 5)
                    throw new Exception("Failed to generate unique PaymentID after multiple attempts.");

                // Tạo entity Payment
                var payment = new Payment
                {
                    PaymentID = paymentId,
                    AccountID = command.AccountID,
                    ClassID = command.ClassID,
                    Total = Convert.ToSingle(classEntity.PriceOfClass),
                    DayCreate = DateTime.UtcNow,
                    Status = PaymentStatus.Pending,
                    TransactionID = null
                };

                var result = await _paymentRepository.CreatePaymentAsync(payment);

                if (!result.Contains("successfully", StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Failed to create payment: {result}");

                var qrCodeUrl = GetQrCodeUrl(paymentId, classEntity.PriceOfClass);

                return new PaymentResponseDTO
                {
                    PaymentID = paymentId,
                    ClassID = command.ClassID,
                    ClassName = classEntity.ClassName,
                    Total = classEntity.PriceOfClass,
                    QRCodeUrl = qrCodeUrl,
                    Status = PaymentStatus.Pending,
                    DayCreate = payment.DayCreate,
                    Description = command.Description ?? $"Payment for {classEntity.ClassName}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating payment for ClassID: {command.ClassID}. Inner: {ex.InnerException?.Message}");
                throw;
            }
        }


        public async Task<PaymentStatusDTO> CheckPaymentStatusAsync(string paymentId)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);

            if (payment == null)
            {
                throw new ArgumentException("Payment not found");
            }

            return new PaymentStatusDTO
            {
                PaymentID = paymentId,
                Status = payment.Status,
                Total = (decimal)payment.Total,
                PaidDate = payment.Status == PaymentStatus.Paid ? payment.DayCreate : null
            };
        }

        public async Task<WebhookResponseDTO> ProcessWebhookAsync(TransactionDTO transaction)
        {
            try
            {
                await SaveTransactionAsync(transaction);

                string paymentId = ExtractPaymentIdFromDescription(transaction.Description);

                if (string.IsNullOrEmpty(paymentId))
                {
                    return new WebhookResponseDTO
                    {
                        Success = false,
                        Message = "Payment ID not found in transaction description"
                    };
                }

                decimal amountIn = transaction.TransferType == "in" ? transaction.TransferAmount : 0;

                bool updated = await UpdatePaymentStatusAsync(paymentId, amountIn);

                if (updated)
                {
                    return new WebhookResponseDTO
                    {
                        Success = true,
                        Message = "Payment processed successfully"
                    };
                }
                else
                {
                    return new WebhookResponseDTO
                    {
                        Success = false,
                        Message = $"Payment not found or already processed. Payment ID: {paymentId}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new WebhookResponseDTO
                {
                    Success = false,
                    Message = $"Error processing webhook: {ex.Message}"
                };
            }
        }

        public async Task<bool> UpdatePaymentStatusAsync(string paymentId, decimal amountReceived)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);

            if (payment == null || payment.Status == PaymentStatus.Paid)
            {
                return false;
            }

            if (Math.Abs((decimal)payment.Total - amountReceived) > 0.01m)
            {
                _logger.LogWarning($"Amount mismatch for payment {paymentId}. Expected: {payment.Total}, Received: {amountReceived}");
            }

            payment.Status = PaymentStatus.Paid;
            await _paymentRepository.UpdatePaymentAsync(payment);

            return true;
        }

        public async Task<Payment> GetPaymentAsync(string paymentId)
        {
            return await _paymentRepository.GetPaymentByIdAsync(paymentId);
        }

        public string GetQrCodeUrl(string paymentId, decimal amount)
        {
            var sepayConfig = _configuration.GetSection("SepaySettings");
            var bankName = sepayConfig["BankName"] ?? "OCB";
            var subAccount = sepayConfig["SubAccount"] ?? "SEPEIC2025";

            return $"https://qr.sepay.vn/img?acc={subAccount}&bank={bankName}&amount={amount*1000}&des=ID_{paymentId}";
        }

        public string GetWebhookUrl()
        {
            string baseUrl = _configuration["ApplicationUrl"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = "https://localhost:7000";
            }

            var webhookEndpoint = _configuration["SepaySettings:WebhookEndpoint"] ?? "/api/webhooks/payment";
            return $"{baseUrl.TrimEnd('/')}{webhookEndpoint}";
        }

        private async Task SaveTransactionAsync(TransactionDTO transaction)
        {
            var transactionEntity = new Transaction
            {
                Gateway = transaction.Gateway,
                TransactionDate = DateTime.ParseExact(transaction.TransactionDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                AccountNumber = transaction.AccountNumber,
                SubAccount = transaction.SubAccount,
                AmountIn = transaction.TransferType == "in" ? transaction.TransferAmount : 0,
                AmountOut = transaction.TransferType == "out" ? transaction.TransferAmount : 0,
                Accumulated = transaction.Accumulated,
                Code = transaction.Code,
                TransactionContent = transaction.Content,
                ReferenceNumber = transaction.ReferenceCode,
                Body = transaction.Description,
                CreatedAt = DateTime.Now
            };

            await _transactionRepository.CreateTransactionAsync(transactionEntity);
        }

        private string ExtractPaymentIdFromDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            string pattern = @"(PM\d{4})";
            Match match = Regex.Match(description, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }
    }
}