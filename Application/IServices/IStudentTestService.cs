using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;

namespace Application.IServices
{
    public interface IStudentTestService
    {
        Task<OperationResult<bool>> ValidateStudentTestExistsAsync(string studentTestID);

        Task<OperationResult<bool>> SubmitStudentTestAsync(string studentID,string testID, List<SectionAnswerDTO> sectionAnswers);
    }
}
