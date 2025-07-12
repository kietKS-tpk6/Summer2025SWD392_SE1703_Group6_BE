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
    public class GetWritingQuestionsByTestIDCommandHandler : IRequestHandler<GetWritingQuestionsByTestIDCommand, OperationResult<List<WritingQuestionWithBaremsDTO>>>
    {
        private readonly IQuestionService _questionService;

        public GetWritingQuestionsByTestIDCommandHandler(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<OperationResult<List<WritingQuestionWithBaremsDTO>>> Handle(GetWritingQuestionsByTestIDCommand request, CancellationToken cancellationToken)
        {
            return await _questionService.GetWritingQuestionsWithBaremsByTestIDAsync(request.TestID);
        }
    }


}
