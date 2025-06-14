using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Shared;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class GetListAccountForManageHandler : IRequestHandler<GetPaginatedAccountListCommand, PagedResult<AccountForManageDTO>>
    {
        private readonly IAccountService _accountService;
        public GetListAccountForManageHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<PagedResult<AccountForManageDTO>> Handle(GetPaginatedAccountListCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountService.GetPaginatedAccountListAsync(request.page, request.pageSize, request.role,request.gender,request.status);
            return result;
        }

    }
}
