using Application.Common.Constants;
using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IStudentMarksService
    {
        Task<OperationResult<GetTestScoresDTO>> GetTestScoresByTestIdAsync(string testId);
        Task<OperationResult<string>> UpdateStudentMarksFromStudentTestAsync(string studentTestId, string assessmentCriteriaId, string classId);
        Task<OperationResult<bool>> UpdateStudentMarksByLecturerAsync(string studentMarkId, decimal mark, string comment, string lecturerId);
        Task<OperationResult<BatchUpdateResultDTO>> BatchUpdateStudentMarksFromStudentTestsAsync(List<StudentTestUpdateDTO> studentTests, string assessmentCriteriaId, string classId);
    }
}