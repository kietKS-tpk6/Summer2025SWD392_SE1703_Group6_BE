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
    public class GetStudentMarksByClassIdQueryHandler : IRequestHandler<GetStudentMarksByClassIdQuery, OperationResult<List<StudentMarksByClassDTO>>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public GetStudentMarksByClassIdQueryHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<List<StudentMarksByClassDTO>>> Handle(GetStudentMarksByClassIdQuery request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.GetStudentMarksByClassIdAsync(request.ClassId);
        }
    }
}
