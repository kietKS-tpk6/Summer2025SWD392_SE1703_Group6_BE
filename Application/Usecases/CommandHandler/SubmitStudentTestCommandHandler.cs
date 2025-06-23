using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;
using MediatR;
using Application.IServices;


namespace Application.Usecases.CommandHandler
{
    public class SubmitStudentTestCommandHandler : IRequestHandler<SubmitStudentTestCommand, OperationResult<bool>>
    {
        private readonly IStudentTestService _studentTestService;

        public SubmitStudentTestCommandHandler(IStudentTestService studentTestService)
        {
            _studentTestService = studentTestService;
        }

        public async Task<OperationResult<bool>> Handle(SubmitStudentTestCommand request, CancellationToken cancellationToken)
        {
            var validate = await _studentTestService.ValidateStudentTestExistsAsync(request.StudentTestID);
            if (!validate.Success)
                return OperationResult<bool>.Fail(validate.Message);

            return await _studentTestService.SubmitStudentTestAsync(request.StudentTestID, request.TestID,request.SectionAnswers);
        }
    }

}
