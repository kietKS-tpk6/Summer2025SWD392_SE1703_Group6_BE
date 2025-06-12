using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs
{
    public class AccountDTO
    {
        public string AccountID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }
        public AccountRole Role { get; set; }
        public AccountStatus Status { get; set; }


    }
}
