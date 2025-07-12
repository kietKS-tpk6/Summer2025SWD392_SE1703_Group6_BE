using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{
    public class DeleteWritingBaremCommand : IRequest<OperationResult<bool>>
    {
        public string WritingBaremID { get; set; }
    }
}
