using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        public string Gateway { get; set; }

        public DateTime TransactionDate { get; set; }

        public string AccountNumber { get; set; }

        public string SubAccount { get; set; }

        public decimal AmountIn { get; set; }

        public decimal AmountOut { get; set; }

        public decimal Accumulated { get; set; }

        public string Code { get; set; }

        public string TransactionContent { get; set; }

        public string ReferenceNumber { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
