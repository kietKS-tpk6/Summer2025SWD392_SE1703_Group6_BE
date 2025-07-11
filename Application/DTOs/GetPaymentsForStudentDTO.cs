using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class GetPaymentsForStudentDTO
    {
        public string PaymentId { get; set; }
        public string ClassName { get; set; }
        public decimal Total { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
