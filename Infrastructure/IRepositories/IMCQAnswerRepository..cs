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
    public interface IMCQAnswerRepository
    {
        Task<OperationResult<bool>> SaveAnswerAsync(string studentTestID, string questionID, List<string> selectedOptionIDs);
        Task<MCQAnswerDTO> GetAnswerAsync(string studentTestID, string questionID);
        Task<MCQAnswer> GetByStudentTestAndQuestionAsync(string studentTestId, string questionId);

    }
}
