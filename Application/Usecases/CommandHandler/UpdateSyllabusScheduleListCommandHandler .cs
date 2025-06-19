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
    public class UpdateSyllabusScheduleListCommandHandler : IRequestHandler<UpdateSyllabusScheduleListCommand, OperationResult<bool>>
    {
        private readonly ISyllabusScheduleService _syllabusScheduleService;

        public UpdateSyllabusScheduleListCommandHandler(ISyllabusScheduleService syllabusScheduleService)
        {
            _syllabusScheduleService = syllabusScheduleService;
        }

        public async Task<OperationResult<bool>> Handle(UpdateSyllabusScheduleListCommand request, CancellationToken cancellationToken)
        {
            var validationResult = _syllabusScheduleService.ValidateTestTypeDuplicatedInInput(request.ScheduleItems);

            if (!validationResult.Success)
            {
                return OperationResult<bool>.Fail(validationResult.Message);
            }
            return await _syllabusScheduleService.UpdateBulkScheduleWithTestAsync(request.SubjectID, request.ScheduleItems);
        }
    }

}
