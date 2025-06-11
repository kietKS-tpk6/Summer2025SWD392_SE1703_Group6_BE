using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using MediatR;
namespace Application.Usecases.Command
{
    public class AssessmentCriteriaSetupCommand : IRequest<OperationResult<int>>
    {
        public int NumberAssessmentCriteria { get; set; }
        public string SubjectID { get; set; }
    }
}
