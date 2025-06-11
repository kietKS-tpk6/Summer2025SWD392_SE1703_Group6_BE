using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.IRepositories;
using Microsoft.AspNetCore.Mvc;
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

                _logger.LogInformation($"Class price from entity: {classEntity.PriceOfClass}");

                // Tạo entity Payment
                var payment = new Payment
                {
                    PaymentID = paymentId,
                    AccountID = command.AccountID,
                    ClassID = command.ClassID,
                    Total = classEntity.PriceOfClass, 
                    DayCreate = DateTime.UtcNow,
                    Status = PaymentStatus.Pending,
                    TransactionID = null
                };

                _logger.LogInformation($"Payment Total after conversion: {payment.Total}");

                var result = await _paymentRepository.CreatePaymentAsync(payment);

                if (!result.Contains("successfully", StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Failed to create payment: {result}");

                

                return new PaymentResponseDTO
                {
                    PaymentID = paymentId,
                    ClassID = command.ClassID,
                    ClassName = classEntity.ClassName,
                    Total = classEntity.PriceOfClass,
                   
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
                Total = payment.Total,
                PaidDate = payment.Status == PaymentStatus.Paid ? payment.DayCreate : null
            };
        }
        public async Task<WebhookResponseDTO> ProcessWebhookAsync(TransactionDTO transaction)
        {
            _logger.LogInformation($"Received webhook for transaction ID: {transaction.Id} with amount: {transaction.TransferAmount} and type: {transaction.TransferType}");
            decimal amountReceived = (transaction.TransferAmount) / 100;
            try
            {
                _logger.LogInformation($"Processing webhook for transaction ID: {transaction.Id}");
                _logger.LogInformation($"Transaction amount: {transaction.TransferAmount}, Type: {transaction.TransferType}");
                _logger.LogInformation($"Transaction content: {transaction.Content}");
                _logger.LogInformation($"Transaction description: {transaction.Description}");

                // Lưu transaction trước
                //var savedTransactionId = await SaveTransactionAsync(transaction);
                //_logger.LogInformation($"Transaction saved with ID: {savedTransactionId}");

                // Extract PaymentID từ nhiều nguồn
                string paymentId = ExtractPaymentIdFromTransaction(transaction);
                _logger.LogInformation($"Extracted Payment ID: {paymentId}");

                if (string.IsNullOrEmpty(paymentId))
                {
                    _logger.LogWarning($"Payment ID not found in transaction. Content: {transaction.Content}, Description: {transaction.Description}");
                    return new WebhookResponseDTO
                    {
                        Success = false,
                        Message = "Payment ID not found in transaction"
                    };
                }
                
                // Chỉ xử lý transaction IN (nhận tiền)
                if (transaction.TransferType?.ToLower() != "in")
                {
                    return new WebhookResponseDTO
                    {
                        Success = true,
                        Message = "Outgoing transaction ignored"
                    };
                }

                

                bool updated = await UpdatePaymentStatusAsync(paymentId, amountReceived);

                if (updated)
                {
                    _logger.LogInformation($"Payment {paymentId} successfully updated to PAID status");
                    return new WebhookResponseDTO
                    {
                        Success = true,
                        Message = "Payment processed successfully"
                    };
                }
                else
                {
                    _logger.LogWarning($"Failed to update payment {paymentId} - may not exist or already processed");
                    return new WebhookResponseDTO
                    {
                        Success = false,
                        Message = $"Payment not found or already processed. Payment ID: {paymentId}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return new WebhookResponseDTO
                {
                    Success = false,
                    Message = $"Error processing webhook: {ex.Message}"
                };
            }
        }

        public async Task<bool> UpdatePaymentStatusAsync(string paymentId, decimal amountReceived)
        {
            _logger.LogInformation($"Attempting to update payment {paymentId} with amount {amountReceived}");
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);

            if (payment == null)
            {
                _logger.LogWarning($"Payment {paymentId} not found");
                return false;
            }

            if (payment.Status == PaymentStatus.Paid)
            {
                _logger.LogInformation($"Payment {paymentId} already marked as paid");
                return false;
            }

            _logger.LogInformation($"Payment found. Expected amount: {payment.Total}, Received: {amountReceived}");
            
            // Kiểm tra số tiền (cho phép sai lệch nhỏ do làm tròn)
            decimal expectedAmount = payment.Total/100;
            if (Math.Abs(expectedAmount  - amountReceived) > 1m) // Cho phép sai lệch 1 VND
            {
                _logger.LogWarning($"Amount mismatch for payment {paymentId}. Expected: {expectedAmount}, Received: {amountReceived}");
                // Vẫn cập nhật nếu số tiền nhận được >= số tiền yêu cầu
                if (amountReceived < expectedAmount)
                {
                    return false;
                }
            }

            payment.Status = PaymentStatus.Paid;
            //if (transactionId.HasValue)
            //{
            //    payment.TransactionID = transactionId.Value;
            //}

            var updateResult = await _paymentRepository.UpdatePaymentAsync(payment);
            _logger.LogInformation($"Payment update result: {updateResult}");

            return updateResult.Contains("successfully", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<Payment> GetPaymentAsync(string paymentId)
        {
            return await _paymentRepository.GetPaymentByIdAsync(paymentId);
        }

        public string GetQrCodeUrl(string paymentId, decimal total)
        {
            var sepayConfig = _configuration.GetSection("SepaySettings");
            var bankName = sepayConfig["BankName"] ?? "OCB";
            var subAccount = sepayConfig["SubAccount"] ?? "SEPEIC2025";

            _logger.LogInformation($"Generating QR for Payment {paymentId} with input amount: {total}");

            var qrUrl = $"https://qr.sepay.vn/img?acc={subAccount}&bank={bankName}&amount={total/100}&des=ID_{paymentId}";

            return qrUrl;
        }
        
        public string GetWebhookUrl()
        {
            string baseUrl = _configuration["ApplicationUrl"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                // Fallback - có thể lấy từ HttpContext nếu cần
                baseUrl = "https://localhost:7000";
            }

            var webhookEndpoint = _configuration["SepaySettings:WebhookEndpoint"] ?? "/api/webhooks/payment";
            Console.WriteLine("webhook link: " + $"{baseUrl.TrimEnd('/')}{webhookEndpoint}");
            return $"{baseUrl.TrimEnd('/')}{webhookEndpoint}";
        }

        private async Task<int> SaveTransactionAsync(TransactionDTO transaction)
        {
            try
            {
                _logger.LogInformation($"start saved transaction with ID: {transaction.Id}");
                var transactionEntity = new Transaction
                {
                    TransactionID = transaction.Id,
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

                var transactionId = await _transactionRepository.CreateTransactionAsync(transactionEntity);
                _logger.LogInformation($"Transaction saved successfully with ID: {transactionId}");
                return transactionId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving transaction");
                throw;
            }
        }

        private string ExtractPaymentIdFromTransaction(TransactionDTO transaction)
        {
            // Thử extract từ Description trước
            string paymentId = ExtractPaymentIdFromDescription(transaction.Description);

            if (!string.IsNullOrEmpty(paymentId))
            {
                return paymentId;
            }

            // Thử extract từ Content
            paymentId = ExtractPaymentIdFromDescription(transaction.Content);

            if (!string.IsNullOrEmpty(paymentId))
            {
                return paymentId;
            }

            // Thử extract từ ReferenceCode
            //paymentId = ExtractPaymentIdFromDescription(transaction.ReferenceCode);

            return paymentId ?? string.Empty;
        }

        private string ExtractPaymentIdFromDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            // Pattern để tìm PM theo sau bởi 4 chữ số
            string pattern = @"PM\d{4}";

            Match match = Regex.Match(description, pattern);
            _logger.LogInformation($"description -------> {description}");
            _logger.LogInformation($"{match.Success}");

            if (match.Success)
            {
                return match.Value;
            }

            return string.Empty;
        }
    }
}