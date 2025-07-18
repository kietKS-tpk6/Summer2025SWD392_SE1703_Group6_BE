﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IWritingAnswerRepository
    {
        Task<OperationResult<bool>> SaveAnswerAsync(string studentTestID, string questionID, string essay);
        Task<WritingAnswer> GetByStudentTestAndQuestionAsync(string studentTestId, string questionId);

    }
}
