using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateSystemConfigCommand : IRequest<OperationResult<bool>>
    {
        public string KeyToUpdate { get; set; }
        public string Value { get; set; }
    }
}
