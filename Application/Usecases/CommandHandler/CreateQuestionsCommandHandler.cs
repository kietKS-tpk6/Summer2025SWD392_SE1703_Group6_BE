using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Common.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Application.Usecases.Command;
namespace Application.Usecases.CommandHandler
{
    public class CreateQuestionsCommandHandler : IRequestHandler<CreateQuestionsCommand, OperationResult<List<Question>>>
    {
        private readonly IQuestionService _questionService;
        private readonly ITestSectionService _testSectionService;

        public CreateQuestionsCommandHandler(IQuestionService questionService, ITestSectionService testSectionService)
        {
            _questionService = questionService;
            _testSectionService = testSectionService;
        }

        public async Task<OperationResult<List<Question>>> Handle(CreateQuestionsCommand request, CancellationToken cancellationToken)
        {
            if (!await _testSectionService.IsTestSectionExistAsync(request.TestSectionID))
                return OperationResult<List<Question>>.Fail("TestSectionID không tồn tại.");

            if (!await _questionService.IsTestFormatTypeConsistentAsync(request.TestSectionID, request.FormatType))
                return OperationResult<List<Question>>.Fail("Các câu hỏi đã tồn tại trong Section này không cùng FormatType.");

            var sectionTypeCheck = await _testSectionService.ValidateSectionTypeMatchFormatAsync(request.TestSectionID, request.FormatType);
            if (!sectionTypeCheck.Success)
                return OperationResult<List<Question>>.Fail(sectionTypeCheck.Message);

            var result = _questionService.ValidateWritingQuestionRule(request.FormatType, request.NumberOfQuestions);
            if (!result.Success)
                return OperationResult<List<Question>>.Fail(result.Message);

            return await _questionService.CreateEmptyQuestionsAsync(request);
        }

    }

}
