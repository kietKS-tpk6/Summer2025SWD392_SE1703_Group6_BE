using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class SearchAccountsQueryCommand : IRequest<OperationResult<List<AccountDTO>>>
    {
        public string SearchValue { get; set; }
        public bool SearchByName { get; set; }
        public bool SearchByGender { get; set; }
        public bool SearchByPhoneNumber { get; set; }
        public bool SearchByEmail { get; set; }
        public bool SearchByRole { get; set; }
        public bool SearchByStatus { get; set; }
    }
}
