using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, OperationResult<bool>>
    {
        private readonly IAccountService _accountService;

        public UpdateAccountCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<OperationResult<bool>> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
        {
            return await _accountService.UpdateAccountAsync(command);
        }
    }
}
