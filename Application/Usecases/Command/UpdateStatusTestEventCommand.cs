using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Enums;
using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateStatusTestEventCommand : IRequest<OperationResult<bool>>
    {
        public string TestEventIDToUpdate { get; set; }
        public TestEventStatus Status { get; set; }
    }
}
