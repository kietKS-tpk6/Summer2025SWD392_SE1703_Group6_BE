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
    public class VerifyOTPCommandHandler : IRequestHandler<VerifyOTPCommand, string>
    {
        private readonly IAccountService _accountService;
        public VerifyOTPCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<string> Handle(VerifyOTPCommand command, CancellationToken cancellationToken)
        {
            var result = await _accountService.VerifyOTPByEmail(command);
            return result ? "Xác thực thành công" : "Xác thực thất bại";
        }
    }
}
