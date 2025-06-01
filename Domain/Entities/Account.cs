using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("Accounts")]
    public class Account
    {
            [Key]
            public string AccountID { get; set; }

            public string LastName { get; set; }
            public string FirstName { get; set; }

            public string Fullname => $"{FirstName} {LastName}"; // Optional: computed property

            public string HashPass { get; set; }

            public Gender Gender { get; set; } // Enum

            [Phone]
            public string PhoneNumber { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public DateOnly BirthDate { get; set; }

            public string? Image { get; set; } // Image field

            public AccountRole Role { get; set; } 
            public AccountStatus Status { get; set; }
            public int FailedLoginAttempts { get; set; }
            public int LastFailedLoginTime {  get; set; }
    }
}
