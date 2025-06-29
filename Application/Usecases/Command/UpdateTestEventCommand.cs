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
    public class UpdateTestEventCommand : IRequest<OperationResult<bool>>
    {
        public string TestEventIdToUpdate { get; set; }
        public string TestID { get; set; }
        public string? Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int AttemptLimit { get; set; }
        public string? Password { get; set; }
    }
}
