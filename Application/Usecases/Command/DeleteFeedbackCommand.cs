﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Usecases.Command
{
    public class DeleteFeedbackCommand : IRequest<bool>
    {
        public string FeedbackID { get; set; }
    }
}
