using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Application.Validators;
using MediatR;
namespace Application.Usecases.CommandHandler
{
    public class AssessmentCriteriaCreateHandler : IRequestHandler<AssessmentCriteriaCreateCommand, bool>
    {
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
        public AssessmentCriteriaCreateHandler(IAssessmentCriteriaService assessmentCriteriaService)
        {
            _assessmentCriteriaService = assessmentCriteriaService;
        }
        public async Task<bool> Handle(AssessmentCriteriaCreateCommand request, CancellationToken cancellationToken)
        {
            return await _assessmentCriteriaService.CreateAssessmentCriteriaAsync(request);
        }

    }
}
