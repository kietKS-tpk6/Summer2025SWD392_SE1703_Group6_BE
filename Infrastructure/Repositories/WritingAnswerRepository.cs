using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;

namespace Infrastructure.Repositories
{
    public class WritingAnswerRepository : IWritingAnswerRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public WritingAnswerRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<bool>> SaveAnswerAsync(string studentTestID, string questionID, string essay)
        {
            try
            {
                var writingAnswer = new WritingAnswer
                {
                    WritingAnswerID = Guid.NewGuid().ToString().Substring(0, 6),
                    StudentTestID = studentTestID,
                    QuestionID = questionID,
                    StudentEssay = essay,
                };

                _dbContext.WritingAnswers.Add(writingAnswer);
                await _dbContext.SaveChangesAsync();

                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail("Lỗi khi lưu WritingAnswer: " + ex.Message);
            }
        }
    }
}
