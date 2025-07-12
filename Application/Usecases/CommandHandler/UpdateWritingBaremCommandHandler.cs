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
    public class UpdateWritingBaremCommandHandler : IRequestHandler<UpdateWritingBaremCommand, OperationResult<bool>>
    {
        private readonly IWritingBaremService _writingBaremService;

        public UpdateWritingBaremCommandHandler(IWritingBaremService writingBaremService)
        {
            _writingBaremService = writingBaremService;
        }

        public async Task<OperationResult<bool>> Handle(UpdateWritingBaremCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _writingBaremService.ValidateUpdateBaremAsync(request);
            if (!validationResult.Success)
                return OperationResult<bool>.Fail(validationResult.Message);

            return await _writingBaremService.UpdateWritingBaremAsync(request);
        }
    }
}
