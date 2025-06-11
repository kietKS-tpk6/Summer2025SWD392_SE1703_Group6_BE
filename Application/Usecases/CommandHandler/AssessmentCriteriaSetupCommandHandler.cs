using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Application.Usecases.Command;
using Application.IServices;
namespace Application.Usecases.CommandHandler
{
    public class AssessmentCriteriaSetupCommandHandler : IRequestHandler<AssessmentCriteriaSetupCommand, string>
    {
        private readonly IAssessmentCriteriaService _assessmentCriteriaService;
        public AssessmentCriteriaSetupCommandHandler(IAssessmentCriteriaService assessmentCriteriaService)
        {
            _assessmentCriteriaService = assessmentCriteriaService;
        }
        public Task<string> Handle(AssessmentCriteriaSetupCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
