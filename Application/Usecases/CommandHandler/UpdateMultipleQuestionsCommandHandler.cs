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
    public class UpdateMultipleQuestionsCommandHandler : IRequestHandler<UpdateMultipleQuestionsCommand, OperationResult<bool>>
    {
        private readonly IQuestionService _questionService;

        public UpdateMultipleQuestionsCommandHandler(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<OperationResult<bool>> Handle(UpdateMultipleQuestionsCommand request, CancellationToken cancellationToken)
        {
            foreach (var command in request.Questions)
            {
                // Validate từng câu hỏi (tách biệt, không xử lý DB)
                var existsResult = await _questionService.ValidateQuestionExistsAsync(command.QuestionID);
                if (!existsResult.Success)
                    return OperationResult<bool>.Fail($"Câu hỏi {command.QuestionID}: {existsResult.Message}");

                //var contentCheck = _questionService.ValidateExactlyOneContent(command.Context, command.ImageURL, command.AudioURL);
                //if (!contentCheck.Success)
                //    return OperationResult<bool>.Fail($"Câu hỏi {command.QuestionID}: {contentCheck.Message}");

                var question = await _questionService.GetQuestionByIdAsync(command.QuestionID);
                if (question.Type == TestFormatType.Multiple || question.Type == TestFormatType.TrueFalse)
                {
                    var optionsCheck = _questionService.ValidateMCQOptions(command.Options);
                    if (!optionsCheck.Success)
                        return OperationResult<bool>.Fail($"Câu hỏi {command.QuestionID}: {optionsCheck.Message}");

                    var duplicateCheck = _questionService.ValidateMCQOptionsNoDuplicate(command.Options);
                    if (!duplicateCheck.Success)
                        return OperationResult<bool>.Fail($"Câu hỏi {command.QuestionID}: {duplicateCheck.Message}");

                    var limitCheck = await _questionService.ValidateOptionCountLimitAsync(command.Options);
                    if (!limitCheck.Success)
                        return OperationResult<bool>.Fail($"Câu hỏi {command.QuestionID}: {limitCheck.Message}");
                }
            }

            // Nếu tất cả đều hợp lệ → gọi service xử lý DB trong transaction
            return await _questionService.UpdateMultipleQuestionsAsync(request.Questions);

        }

    }
}
