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
        Task<OperationResult<string>> CreateStudentMarkFromStudentTestAsync(string studentTestId);
        Task<OperationResult<bool>> DeleteStudentMarkAsync(string studentMarkId);
        Task<OperationResult<List<StudentMarkDTO>>> GetStudentMarksByClassAndAssessmentAsync(string classId, string assessmentCriteriaId);
        Task<OperationResult<List<StudentMarkDTO>>> GetStudentMarksByStudentIdAsync(string studentId);
        //Kho - Setup bảng điểm khi class chốt sĩ số
        Task<OperationResult<bool>> SetupStudentMarkByClassIdAsync(string classId);
        //Kho - Get bảng điểm theo class
        Task<OperationResult<List<StudentMarkDetailDTO>>> GetStudentMarkDetailDTOByClassIdAsync(string classId);
    }
}