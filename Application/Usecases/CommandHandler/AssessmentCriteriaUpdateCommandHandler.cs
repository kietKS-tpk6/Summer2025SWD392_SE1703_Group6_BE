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
    public class AssessmentCriteriaUpdateCommandHandler: IRequestHandler<AssessmentCriteriaUpdateCommand, string>
    {
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
        public AssessmentCriteriaUpdateCommandHandler(IAssessmentCriteriaService assessmentCriteriaService)
        {
            _assessmentCriteriaService = assessmentCriteriaService;
        }
        public async Task<string> Handle(AssessmentCriteriaUpdateCommand request, CancellationToken cancellationToken)
        {
            return await _assessmentCriteriaService.UpdateAssessmentCriteriaAsync(request);
        }
    }
}
