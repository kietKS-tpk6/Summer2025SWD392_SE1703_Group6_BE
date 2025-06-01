using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Usecases.Command;
namespace Application.IServices
{
    public interface IAssessmentCriteriaService
    {
        public Task<bool> CreateAssessmentCriteriaAsync(AssessmentCriteriaCreateCommand request);
    }
}
