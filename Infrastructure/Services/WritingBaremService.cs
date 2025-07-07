using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Infrastructure.IRepositories;
namespace Infrastructure.Services
{
    public class WritingBaremService : IWritingBaremService
    {
        private readonly IWritingBaremRepository _writingBaremRepository;
        private readonly IQuestionRepository _questionRepository;

        public WritingBaremService(IWritingBaremRepository writingBaremRepository, IQuestionRepository questionRepository)
        {
            _writingBaremRepository = writingBaremRepository;
            _questionRepository = questionRepository;
        }

        public async Task<OperationResult<bool>> ValidateCreateBaremsAsync(List<CreateWritingBaremDTO> barems)
        {
            var questionIDs = barems.Select(b => b.QuestionID).Distinct().ToList();
            var validQuestions = await _questionRepository.GetByIDsAsync(questionIDs);
            var validIDs = validQuestions.Select(q => q.QuestionID).ToHashSet();

            var invalidIDs = questionIDs.Where(q => !validIDs.Contains(q)).ToList();

            if (invalidIDs.Any())
                return OperationResult<bool>.Fail($"Không tìm thấy QuestionID: {string.Join(", ", invalidIDs)}");

            return OperationResult<bool>.Ok(true);
        }

        public async Task<OperationResult<bool>> CreateWritingBaremsAsync(List<CreateWritingBaremDTO> barems)
        {
            var entities = barems.Select(b => new WritingBarem
            {
                WritingBaremID = Guid.NewGuid().ToString("N")[..6],
                QuestionID = b.QuestionID,
                CriteriaName = b.CriteriaName,
                MaxScore = b.MaxScore,
                Description = b.Description
            }).ToList();

            await _writingBaremRepository.AddRangeAsync(entities);
            return OperationResult<bool>.Ok(true, "Tạo thành công.");
        }
        public async Task<OperationResult<List<WritingBaremDTO>>> GetByQuestionIDAsync(string questionID)
        {
            var barems = await _writingBaremRepository.GetByQuestionIDAsync(questionID);

            var result = barems.Select(b => new WritingBaremDTO
            {
                WritingBaremID = b.WritingBaremID,
                CriteriaName = b.CriteriaName,
                MaxScore = b.MaxScore,
                Description = b.Description,
            }).ToList();

            return OperationResult<List<WritingBaremDTO>>.Ok(result);
        }
    }

}
