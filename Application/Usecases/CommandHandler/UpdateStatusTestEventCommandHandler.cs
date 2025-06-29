using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class UpdateStatusTestEventCommandHandler : IRequestHandler<UpdateStatusTestEventCommand, OperationResult<bool>>
    {
        private readonly ITestEventService _testEventService;
        public UpdateStatusTestEventCommandHandler(ITestEventService testEventService)
        {
            _testEventService = testEventService;
        }
        public async Task<OperationResult<bool>> Handle(UpdateStatusTestEventCommand request, CancellationToken cancellationToken)
        {
            return await _testEventService.UpdateStatusAsync(request);
        }
    }
}
