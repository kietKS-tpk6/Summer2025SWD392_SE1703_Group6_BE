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
    public class GetStudentTestResultByStudentTestIDQueryCommandHandler
    : IRequestHandler<GetStudentTestResultByStudentTestIDQueryCommand, OperationResult<StudentTestResultDTO>>
    {
        private readonly ITestService _testService;

        public GetStudentTestResultByStudentTestIDQueryCommandHandler(ITestService testService)
        {
            _testService = testService;
        }

        public async Task<OperationResult<StudentTestResultDTO>> Handle(GetStudentTestResultByStudentTestIDQueryCommand request, CancellationToken cancellationToken)
        {
            return await _testService.GetStudentTestResultByStudentTestIDAsync(request.StudentTestID);
        }
    }
}
