using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class AssessmentCriteriaUpdateCommandHandler: IRequestHandler<AssessmentCriteriaUpdateCommand, OperationResult<bool>>
    {
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
        public AssessmentCriteriaUpdateCommandHandler(IAssessmentCriteriaService assessmentCriteriaService)
        {
            _assessmentCriteriaService = assessmentCriteriaService;
        }
        public async Task<OperationResult<bool>> Handle(AssessmentCriteriaUpdateCommand request, CancellationToken cancellationToken)
        {
           
            return await _assessmentCriteriaService.UpdateAssessmentCriteriaAsync(request);
        }
    }
}
