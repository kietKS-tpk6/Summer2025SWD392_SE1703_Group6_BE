using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class SendOTPViaEmailCommandHandler : IRequestHandler<SendOTPViaEmailCommand, bool>
    {
        private readonly IEmailService _emailService;
        public SendOTPViaEmailCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task<bool> Handle(SendOTPViaEmailCommand req, CancellationToken cancellationToken)
        {
            var res = await _emailService.SendOtpEmailAsync(req.Email);  
            return res;
        }
    }
}
