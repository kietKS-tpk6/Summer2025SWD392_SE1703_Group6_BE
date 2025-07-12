using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PaymentTableRowDTO
    {
        public string PaymentID { get; set; }
        public string ClassName { get; set; }
        public string StudentName { get; set; }
        public decimal  Amount { get; set; }
        public string Status { get; set; }
        public DateTime PaidAt { get; set; }
    }

}
