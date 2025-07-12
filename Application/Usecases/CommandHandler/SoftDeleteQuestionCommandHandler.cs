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
    public class SoftDeleteQuestionCommandHandler : IRequestHandler<SoftDeleteQuestionCommand, OperationResult<bool>>
    {
        private readonly IQuestionService _questionService;

        public SoftDeleteQuestionCommandHandler(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<OperationResult<bool>> Handle(SoftDeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            return await _questionService.SoftDeleteQuestionAsync(request.QuestionID);
        }
    }
}
