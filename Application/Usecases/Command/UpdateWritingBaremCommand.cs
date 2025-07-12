using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;

namespace Application.Usecases.Command
{

    public class UpdateWritingBaremCommand : IRequest<OperationResult<bool>>
    {
        public string WritingBaremID { get; set; }
        public string CriteriaName { get; set; }
        public decimal MaxScore { get; set; }
        public string? Description { get; set; }

    }

}
