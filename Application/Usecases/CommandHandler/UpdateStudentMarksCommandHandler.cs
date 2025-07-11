using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;
using Application.Usecases.Command;
using Application.IServices;
using Application.DTOs;
namespace Application.Usecases.CommandHandler
{
    public class UpdateStudentMarksCommandHandler : IRequestHandler<UpdateStudentMarksCommand, OperationResult<bool>>
    {
        private readonly IStudentMarksService _studentMarksService;
        private readonly IAccountService _accountService;

        public UpdateStudentMarksCommandHandler(IStudentMarksService studentMarksService, IAccountService accountService)
        {
            _studentMarksService = studentMarksService;
            _accountService = accountService;
        }
        public async Task<OperationResult<bool>> Handle(UpdateStudentMarksCommand request, CancellationToken cancellationToken)
        {
            var accountFound = await _accountService.GetAccountByIdAsync(request.LecturerId);
            if (!accountFound.Success)
            {
                return OperationResult<bool>.Fail(OperationMessages.NotFound("giảng viên"));
            }
            return await _studentMarksService.UpdateStudentMarksAsync(request);
        }
    }
}
