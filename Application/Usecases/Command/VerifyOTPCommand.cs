using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Usecases.Command
{
    public class VerifyOTPCommand : IRequest<string>
    {
        public string Email { get; set; }

        public string OTP { get; set; }
    }
}
