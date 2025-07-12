using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
