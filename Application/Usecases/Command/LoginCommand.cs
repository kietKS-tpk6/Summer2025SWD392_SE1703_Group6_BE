using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class LoginCommand : IRequest<LoginDTO>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
