using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Queries;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.QueryHandlers
{
    public class GetStudentMarksByStudentIdQueryHandler : IRequestHandler<GetStudentMarksByStudentIdQuery, OperationResult<List<StudentMarkDTO>>>
    {
        private readonly IStudentMarksService _studentMarksService;

        public GetStudentMarksByStudentIdQueryHandler(IStudentMarksService studentMarksService)
        {
            _studentMarksService = studentMarksService;
        }

        public async Task<OperationResult<List<StudentMarkDTO>>> Handle(GetStudentMarksByStudentIdQuery request, CancellationToken cancellationToken)
        {
            return await _studentMarksService.GetStudentMarksByStudentIdAsync(request.StudentId);
        }
    }
}