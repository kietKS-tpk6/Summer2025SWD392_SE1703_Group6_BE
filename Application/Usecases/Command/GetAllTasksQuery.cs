﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Entities;
using MediatR;

namespace Application.Usecases.Query
{
    public class GetAllTasksQuery : IRequest<OperationResult<List<WorkTask>>>
    {
    }
}