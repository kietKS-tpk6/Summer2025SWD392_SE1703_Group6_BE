using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MCQAnswerRepository : IMCQAnswerRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public MCQAnswerRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MCQAnswerDTO> GetAnswerAsync(string studentTestID, string questionID)
        {
            var mcqAnswer = await _dbContext.MCQAnswers
                .FirstOrDefaultAsync(a => a.StudentTestID == studentTestID && a.QuestionID == questionID);

            if (mcqAnswer == null) return null;

            var optionIDs = await _dbContext.MCQAnswerDetails
                .Where(d => d.MCQAnswerID == mcqAnswer.MCQAnswerID)
                .Select(d => d.MCQOptionID)
                .ToListAsync();

            return new MCQAnswerDTO
            {
                SelectedOptionIDs = optionIDs
            };
        }


        public async Task<OperationResult<bool>> SaveAnswerAsync(string studentTestID, string questionID, List<string> selectedOptionIDs)
        {
            try
            {
                var mcqAnswer = new MCQAnswer
                {
                    MCQAnswerID = Guid.NewGuid().ToString().Substring(0, 6),
                    StudentTestID = studentTestID,
                    QuestionID = questionID
                };
                _dbContext.MCQAnswers.Add(mcqAnswer);
                await _dbContext.SaveChangesAsync();
                foreach (var optionID in selectedOptionIDs)
                {
                    var detail = new MCQAnswerDetail
                    {
                        MCQAnswerDetailID = Guid.NewGuid().ToString().Substring(0, 6),
                        MCQAnswerID = mcqAnswer.MCQAnswerID,
                        MCQOptionID = optionID 
                    };
                    _dbContext.MCQAnswerDetails.Add(detail);
                }

                await _dbContext.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail("Lỗi lưu MCQAnswer: " + ex.Message);
            }
        }
    
    }
}
