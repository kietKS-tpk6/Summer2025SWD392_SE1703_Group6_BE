using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IStudentMarkRepository
    {
        Task<StudentMark> GetByIdAsync(string id);
        Task<StudentMark> GetByStudentAndAssessmentCriteriaAsync(string studentId, string assessmentCriteriaId, string classId);
        Task<StudentMark> CreateAsync(StudentMark studentMarks);
        Task<StudentMark> UpdateAsync(StudentMark studentMarks);
        Task<bool> DeleteAsync(string id);
        Task<List<StudentMark>> GetByClassIdAsync(string classId);
        Task<List<StudentMark>> GetByAssessmentCriteriaAndClassAsync(string assessmentCriteriaId, string classId);
        Task<List<StudentMark>> GetByStudentIdAsync(string studentId);
        Task<int> CountAsync();
        Task<List<StudentMark>> GetByStudentTestIdAsync(string studentTestId);
        //Kho - setup bảng điểm null
        Task<OperationResult<bool>> SetupStudentMarkByClassIdAsync(string classId);
        //Kho - get bảng điểm theo lớp
        Task<OperationResult<List<StudentMarkDetailKhoDTO>>> GetStudentMarkDetailDTOByClassIdAsync(string classId);
        Task<List<StudentMark>> GetMarksByStudentAndClassAsync(string studentID, string classID);
        Task<List<StudentMark>> GetMarksByClassAsync(string classId);

    }
}
