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
    public class GradeWritingAnswerCommandHandler : IRequestHandler<GradeWritingAnswerCommand, OperationResult<bool>>
    {
        private readonly IStudentTestService _studentTestService;

        public GradeWritingAnswerCommandHandler(IStudentTestService studentTestService)
        {
            _studentTestService = studentTestService;
        }

        public async Task<OperationResult<bool>> Handle(GradeWritingAnswerCommand request, CancellationToken cancellationToken)
        {
            var validateResult = await _studentTestService.ValidateWritingScoreAsync(request.TestSectionID, request.WritingScore);
            if (!validateResult.Success)
                return validateResult;

            return await _studentTestService.GradeWritingAnswerAsync(request);
        }
    }

}
