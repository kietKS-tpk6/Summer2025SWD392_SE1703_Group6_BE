using Application.Common.Constants;
using Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Application.Usecases.Queries
{
    public class GetStudentMarksByStudentIdQuery : IRequest<OperationResult<List<StudentMarkDTO>>>
    {
        public string StudentId { get; set; }
    }
}