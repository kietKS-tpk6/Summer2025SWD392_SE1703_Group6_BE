using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Application.Usecases.Command;
using Application.IServices;
using Application.Common.Constants;
using Application.DTOs;
namespace Application.Usecases.CommandHandler
{
    public class AssessmentCriteriaSetupCommandHandler : IRequestHandler<AssessmentCriteriaSetupCommand, OperationResult<List<AssessmentCriteriaSetupDTO>>>
    {
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
        public AssessmentCriteriaSetupCommandHandler(IAssessmentCriteriaService assessmentCriteriaService)
        {
            _assessmentCriteriaService = assessmentCriteriaService;
        }
        public async Task<OperationResult<List<AssessmentCriteriaSetupDTO>>> Handle(AssessmentCriteriaSetupCommand request, CancellationToken cancellationToken)
        {
           return await  _assessmentCriteriaService.SetupAssessmentCriteria(request);
        }
    }
}
