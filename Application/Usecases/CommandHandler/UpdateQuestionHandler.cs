using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class UpdateQuestionHandler : IRequestHandler<UpdateQuestionCommand, OperationResult<bool>>
    {
        private readonly IQuestionService _questionService;

        public UpdateQuestionHandler(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<OperationResult<bool>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            var existsResult = await _questionService.ValidateQuestionExistsAsync(request.QuestionID);
            if (!existsResult.Success) return existsResult;

            var contentCheck = _questionService.ValidateExactlyOneContent(request.Context, request.ImageURL, request.AudioURL);
            if (!contentCheck.Success) return contentCheck;

            // Check đáp án nếu là MCQ hoặc TrueFalse
            var question = await _questionService.GetQuestionByIdAsync(request.QuestionID);
            if (question.Type == TestFormatType.Multiple || question.Type == TestFormatType.TrueFalse)
            {
                var optionsCheck = _questionService.ValidateMCQOptions(request.Options);
                if (!optionsCheck.Success) return optionsCheck;
            }

            return await _questionService.UpdateQuestionAsync(request);
        }
    }

}
