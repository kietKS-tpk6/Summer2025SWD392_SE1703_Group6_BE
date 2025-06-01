using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OTP
    {
        [Key]
        public int OTPID { get; set; }

        [ForeignKey("Account")]
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [ForeignKey("Account")]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool IsUsed { get; set; }

        public virtual Account Account { get; set; }
    }
}
