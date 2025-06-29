using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using MediatR;
using Application.Usecases.Command;
namespace Application.Usecases.CommandHandler
{
    public class UpdateTestEventCommandHandler: IRequestHandler<UpdateTestEventCommand, OperationResult<bool>>
    {
        private readonly ITestEventService _testEventService;

        public UpdateTestEventCommandHandler(ITestEventService testEventService)
        {
            _testEventService = testEventService;
        }

        public async Task<OperationResult<bool>> Handle(UpdateTestEventCommand request,CancellationToken cancellationToken)
        {
            return await _testEventService.UpdateTestEventAsync(request);
        }
    }
}
