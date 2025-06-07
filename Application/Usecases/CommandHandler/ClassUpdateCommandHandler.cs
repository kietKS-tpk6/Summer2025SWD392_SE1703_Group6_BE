using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class ClassUpdateCommandHandler : IRequestHandler<ClassUpdateCommand, bool>
    {
        private readonly IClassService _classService;

        public ClassUpdateCommandHandler(IClassService classService)
        {
            _classService = classService;
        }
        public async Task<bool> Handle(ClassUpdateCommand request, CancellationToken cancellationToken)
        {
            return await _classService.UpdateClassAsync(request);
        }
    }
}
