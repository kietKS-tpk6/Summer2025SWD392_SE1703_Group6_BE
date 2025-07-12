using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Entities;
using MediatR;


namespace Application.Usecases.CommandHandler
{
    public class CreateWritingBaremsCommandHandler : IRequestHandler<CreateWritingBaremsCommand, OperationResult<bool>>
    {
        private readonly IWritingBaremService _writingBaremService;

        public CreateWritingBaremsCommandHandler(IWritingBaremService writingBaremService)
        {
            _writingBaremService = writingBaremService;
        }

        public async Task<OperationResult<bool>> Handle(CreateWritingBaremsCommand request, CancellationToken cancellationToken)
        {
            if (request.Barems == null || !request.Barems.Any())
                return OperationResult<bool>.Fail("Danh sách không được rỗng.");

            var validationResult = await _writingBaremService.ValidateCreateBaremsAsync(request.Barems);
            if (!validationResult.Success)
                return validationResult;
            var validationScoreResult = await _writingBaremService.ValidateWritingBaremsAsync(request.Barems);
            if (!validationScoreResult.Success)
                return validationScoreResult;

            // Gọi service xử lý
            return await _writingBaremService.CreateWritingBaremsAsync(request.Barems);
        }
    }

}
