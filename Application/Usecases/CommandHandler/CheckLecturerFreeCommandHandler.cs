using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Application.Usecases.Command;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
namespace Application.Usecases.CommandHandler
{
    public class CheckLecturerFreeCommandHandler : IRequestHandler<CheckLecturerFreeCommand, OperationResult<List<AccountDTO>>>
    {
        private readonly IAccountService _accountService;
        public CheckLecturerFreeCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<OperationResult<List<AccountDTO>>> Handle(CheckLecturerFreeCommand request, CancellationToken cancellationToken)
        {
            return await _accountService.GetFreeLecturersAsync(request);
        }
    }
}
