using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Application.Usecases.Command;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;

namespace Application.Usecases.CommandHandler
{
    public class GetStudentMarkForStudentCommandHandler: IRequestHandler<GetStudentMarkForStudentCommand, OperationResult<StudentMarkForStudentDTO>>
    {
        private readonly IStudentMarksService _studentMarksService;
        private readonly IClassService _classService;
        private readonly IAccountService _accountService;
        private readonly IEnrollmentService _enrollmentService;
        public GetStudentMarkForStudentCommandHandler(IStudentMarksService studentMarksService, IClassService classService, 
            IAccountService accountService, IEnrollmentService enrollmentService)
        {
            _studentMarksService = studentMarksService;
            _classService = classService;
            _accountService = accountService;
            _enrollmentService = enrollmentService;
        }
        public async Task<OperationResult<StudentMarkForStudentDTO>> Handle(GetStudentMarkForStudentCommand request, CancellationToken cancellationToken)
        {
            var accountFound = await _accountService.GetAccountByIdAsync(request.StudentId);
            if(!accountFound.Success)
            {
                return OperationResult<StudentMarkForStudentDTO>.Fail(OperationMessages.NotFound("sinh viên"));
            }
            var classFound = await _classService.GetClassDTOByIDAsync(request.ClassId);
            if(!classFound.Success)
            {
                return OperationResult<StudentMarkForStudentDTO>.Fail(OperationMessages.NotFound("lớp học"));
            }
            return await _studentMarksService.GetStudentMarkForStudent(request);
        }
    }
}
