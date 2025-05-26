using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Account
    {
            [Key]
            public string AccountID { get; set; }

            public string LastName { get; set; }
            public string FirstName { get; set; }

            public string Fullname => $"{FirstName} {LastName}"; // Optional: computed property

            public string PasswordHash { get; set; }

            public Gender Gender { get; set; } // Enum

            [Phone]
            public string PhoneNumber { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public DateOnly Birthday { get; set; }

            //public AccountStatus Status { get; set; } // Enum

            public string? Avatar { get; set; } // Image field

            public AccountRole Role { get; set; } // Enum

            public int FailedLoginAttempts { get; set; }

            public DateTime? LastFailedLoginTime { get; set; }

            //public List<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
