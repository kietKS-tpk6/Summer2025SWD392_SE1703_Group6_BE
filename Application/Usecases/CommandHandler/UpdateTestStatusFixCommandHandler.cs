using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;
using Application.Usecases.Command;
using Application.IServices;
namespace Application.Usecases.CommandHandler
{
    public class UpdateTestStatusFixCommandHandler: IRequestHandler<UpdateTestStatusFixCommand, OperationResult<string>>
    {
        private readonly ITestService _testService;
        public UpdateTestStatusFixCommandHandler(ITestService testService)
        {
            _testService = testService;
        }
        public async Task<OperationResult<string>> Handle(UpdateTestStatusFixCommand request, CancellationToken cancellationToken)
        {
            return await _testService.UpdateTestStatusFixAsync(request);
        }
    }
}
