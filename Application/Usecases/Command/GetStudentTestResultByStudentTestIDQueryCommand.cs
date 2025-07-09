using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{

    public class GetStudentTestResultByStudentTestIDQueryCommand : IRequest<OperationResult<StudentTestResultDTO>>
    {
        public string StudentTestID { get; set; }

        public GetStudentTestResultByStudentTestIDQueryCommand(string studentTestID)
        {
            StudentTestID = studentTestID;
        }
    }
}
