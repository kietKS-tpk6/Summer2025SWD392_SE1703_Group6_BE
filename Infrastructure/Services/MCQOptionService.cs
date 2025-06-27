using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Domain.Entities;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class MCQOptionService : IMCQOptionService
    {

        private readonly IMCQOptionRepository _mcqOptionRepository;

        public MCQOptionService(IMCQOptionRepository mcqOptionRepository)
        {
            _mcqOptionRepository = mcqOptionRepository;
        }
        public async Task<OperationResult<List<MCQOption>>> GetOptionsByQuestionIDAsync(string questionID)
        {
            //if (string.IsNullOrWhiteSpace(questionID))
            //{
            //    return OperationResult<List<MCQOption>>.Fail("Question ID không được để trống.");
            //}


            var options = await _mcqOptionRepository.GetByQuestionIdAsync(questionID);

            return OperationResult<List<MCQOption>>.Ok(options);
        }
    }
}
