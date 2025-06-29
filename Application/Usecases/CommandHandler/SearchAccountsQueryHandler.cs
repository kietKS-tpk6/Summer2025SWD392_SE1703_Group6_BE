using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class SearchAccountsQueryHandler : IRequestHandler<SearchAccountsQueryCommand, OperationResult<List<AccountDTO>>>
    {
        private readonly IAccountService _accountService;
        public SearchAccountsQueryHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<OperationResult<List<AccountDTO>>> Handle(SearchAccountsQueryCommand command, CancellationToken cancellationToken)
        {
            return await _accountService.SearchAccountsAsync(command);
        }
    }
}
