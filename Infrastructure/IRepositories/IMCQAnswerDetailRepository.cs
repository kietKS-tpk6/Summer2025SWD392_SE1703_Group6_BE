﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IMCQAnswerDetailRepository
    {
        Task<List<MCQAnswerDetail>> GetByMCQAnswerIdAsync(string mcqAnswerId);

    }
}
