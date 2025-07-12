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
    public class ClassUpdateStatusCommandHandler : IRequestHandler<ClassUpdateStatusCommand, OperationResult<bool>>
    {
        private readonly IClassService _classService;
        public ClassUpdateStatusCommandHandler(IClassService classService)
        {
            _classService = classService;
        }
        public async Task<OperationResult<bool>> Handle(ClassUpdateStatusCommand request, CancellationToken cancellationToken)
        {
            var classFound = await _classService.GetClassDTOByIDAsync(request.ClassId);
            if(!classFound.Success || classFound == null)
            {
                return OperationResult<bool>.Fail(OperationMessages.NotFound("lớp học"));
            }
            return await _classService.UpdateStatusAsync(request);
        }
    }
}
