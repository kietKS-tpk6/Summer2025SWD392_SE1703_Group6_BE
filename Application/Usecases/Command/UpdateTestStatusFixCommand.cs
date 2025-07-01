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
    public class UpdateTestStatusFixCommand : IRequest<OperationResult<string>>
    {
        public string TestID {  get; set; }
        public TestStatus TestStatus { get; set; }
    }
}
