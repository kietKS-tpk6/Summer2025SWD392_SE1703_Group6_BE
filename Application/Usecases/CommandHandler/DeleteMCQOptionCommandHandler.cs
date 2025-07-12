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
    public class DeleteMCQOptionCommandHandler : IRequestHandler<DeleteMCQOptionCommand, OperationResult<bool>>
    {
        private readonly IQuestionService _questionService;

        public DeleteMCQOptionCommandHandler(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<OperationResult<bool>> Handle(DeleteMCQOptionCommand request, CancellationToken cancellationToken)
        {
            return await _questionService.DeleteMCQOptionAsync(request.QuestionID, request.MCQOptionID);
        }
    }
}
