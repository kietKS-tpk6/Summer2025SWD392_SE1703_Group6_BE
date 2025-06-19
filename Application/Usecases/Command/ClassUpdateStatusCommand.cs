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
    public class ClassUpdateStatusCommand : IRequest<OperationResult<bool>>
    {
        public string ClassId { get; set; }
        public ClassStatus ClassStatus { get; set; }
    }
}
