using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Shared;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class GetPaginatedAccountListCommand : IRequest<PagedResult<AccountForManageDTO>>
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string? role { get; set; }
        public string? gender { get; set; }
        public string? status { get; set; }

    }
}
