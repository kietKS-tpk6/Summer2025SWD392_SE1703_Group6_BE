using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IStudentTestService
    {

        Task<OperationResult<bool>> SubmitStudentTestAsync(string studentID,string testID, List<SectionAnswerDTO> sectionAnswers);
        Task<OperationResult<bool>> ValidStudentGetExamAsync(string testEventId, string accountId);
        Task<OperationResult<bool>> GradeWritingAnswerAsync(GradeWritingAnswerCommand request);
        Task<OperationResult<bool>> ValidateWritingScoreAsync(string testSectionID, decimal writingScore);
        Task<OperationResult<int>> CountPendingWrittenGradingAsync(string lecturerId);
        Task<OperationResult<List<StudentTestResultSimpleDTO>>> GetSimpleStudentTestsByTestEventAsync(string testEventId);

    }
}
