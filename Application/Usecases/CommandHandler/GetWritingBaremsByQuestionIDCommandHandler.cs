using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class GetWritingBaremsByQuestionIDCommandHandler : IRequestHandler<GetWritingBaremsByQuestionIDCommand, OperationResult<List<WritingBaremDTO>>>
    {
        private readonly IWritingBaremService _writingBaremService;

        public GetWritingBaremsByQuestionIDCommandHandler(IWritingBaremService writingBaremService)
        {
            _writingBaremService = writingBaremService;
        }

        public async Task<OperationResult<List<WritingBaremDTO>>> Handle(GetWritingBaremsByQuestionIDCommand request, CancellationToken cancellationToken)
        {
            return await _writingBaremService.GetByQuestionIDAsync(request.QuestionID);
        }
    }

}
