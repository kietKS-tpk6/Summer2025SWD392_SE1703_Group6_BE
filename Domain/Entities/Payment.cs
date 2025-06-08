using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
namespace Domain.Entities
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        [MaxLength(6)]
        public string PaymentID { get; set; }

        [ForeignKey("Account")]
        [MaxLength(6)]
        public string AccountID { get; set; }

        [ForeignKey("Transaction")]
        public int ?TransactionID { get; set; }

        [ForeignKey("Class")]
        [MaxLength(6)]
        public string ClassID { get; set; }

        public double Total { get; set; }

        public DateTime DayCreate { get; set; }

        public PaymentStatus Status { get; set; }

        public virtual Account Account { get; set; }

        public virtual Transaction Transaction { get; set; }
        public virtual Class Class { get; set; }
    }
}
