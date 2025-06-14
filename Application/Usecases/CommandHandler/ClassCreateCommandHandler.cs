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
    public class ClassCreateCommandHandler : IRequestHandler<ClassCreateCommand, OperationResult<bool>>
    {
        private readonly IClassService _classService;

        public ClassCreateCommandHandler(IClassService classService)
        {
            _classService = classService;
        }
        public async Task<OperationResult<bool>> Handle(ClassCreateCommand request, CancellationToken cancellationToken)
        {
            return await _classService.CreateClassAsync(request);
        }


    }
}
