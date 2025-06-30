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
    public class UpdateSystemConfigCommandHandler : IRequestHandler<UpdateSystemConfigCommand , OperationResult<bool>>
    {
        private readonly ISystemConfigService _systemConfigService;
        public UpdateSystemConfigCommandHandler(ISystemConfigService systemConfigService)
        {
            _systemConfigService = systemConfigService;
        }
        public async Task<OperationResult<bool>> Handle(UpdateSystemConfigCommand request, CancellationToken cancellationToken)
        {
            return await _systemConfigService.UpdateSystemConfigAsync(request);
        }
    }
}
