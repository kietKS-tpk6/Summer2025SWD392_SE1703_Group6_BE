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
    public class AssessmentCriteriaUpdateListCommandHandler : IRequestHandler<AssessmentCriteriaUpdateListCommand, OperationResult<List<AssessmentCriteriaUpdateDTO>>>
    {
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;

        public AssessmentCriteriaUpdateListCommandHandler(IAssessmentCriteriaService service)
        {
            _assessmentCriteriaService = service;
        }

        public async Task<OperationResult<List<AssessmentCriteriaUpdateDTO>>> Handle(AssessmentCriteriaUpdateListCommand request, CancellationToken cancellationToken)
        {
            var check = _assessmentCriteriaService.CheckDuplicateCategory(request.Items);
            if (!check.Success)
                return OperationResult<List<AssessmentCriteriaUpdateDTO>>.Fail(check.Message);

            var countCheck = _assessmentCriteriaService.CheckRequiredTestCountRule(request.Items);
            if (!countCheck.Success)
                return OperationResult<List<AssessmentCriteriaUpdateDTO>>.Fail(countCheck.Message);

            return await _assessmentCriteriaService.UpdateAssessmentCriteriaListAsync(request.Items);
        }

    }
}
