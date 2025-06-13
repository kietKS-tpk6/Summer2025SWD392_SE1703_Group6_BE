using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using MediatR;

namespace Application.Usecases.Command
{
    public class CreateAccountCommand : IRequest<bool>
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; } 
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Role { get; set; }
    }
}
