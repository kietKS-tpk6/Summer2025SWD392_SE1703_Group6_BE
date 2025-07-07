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
    public class CreateWritingBaremsCommand : IRequest<OperationResult<bool>>
    {
        public List<CreateWritingBaremDTO> Barems { get; set; }
    }
}
