using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;

namespace Application.IServices
{
    public interface IMCQOptionService
    {
        Task<OperationResult<List<MCQOption>>> GetOptionsByQuestionIDAsync(string questionID);

    }
}
