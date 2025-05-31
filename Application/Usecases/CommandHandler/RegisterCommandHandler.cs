using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Usecases.Command;
using Application.DTOs;

using MediatR;
using Application.IServices;

namespace Application.Usecases.CommandHandler
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand,RegisterDTO>
    {
        private readonly IAccountService _accountService;
        public RegisterCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<RegisterDTO> Handle(RegisterCommand req, CancellationToken cancellationToken)
        {
            var res= await _accountService.Register(req);
            return res;
        }

    }
}
