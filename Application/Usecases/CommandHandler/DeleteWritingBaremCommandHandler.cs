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
    public class DeleteWritingBaremCommandHandler : IRequestHandler<DeleteWritingBaremCommand, OperationResult<bool>>
    {
        private readonly IWritingBaremService _writingBaremService;

        public DeleteWritingBaremCommandHandler(IWritingBaremService writingBaremService)
        {
            _writingBaremService = writingBaremService;
        }

        public async Task<OperationResult<bool>> Handle(DeleteWritingBaremCommand request, CancellationToken cancellationToken)
        {
            return await _writingBaremService.SoftDeleteWritingBaremAsync(request.WritingBaremID);
        }
    }
}
