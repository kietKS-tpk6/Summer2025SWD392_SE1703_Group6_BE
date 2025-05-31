using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;
using Application.Usecases.Command;

using Application.IServices;
namespace Application.Usecases.CommandHandler
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand , LoginDTO>
    {
        private readonly IAccountService _accountService;
        public LoginCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<LoginDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _accountService.Login(request);
            return user;
        }

    }
}
