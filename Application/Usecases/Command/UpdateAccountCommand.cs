using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateAccountCommand : IRequest<OperationResult<AccountDTO>>
    {
        public string AccountID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? Img { get; set; }
    }

}
