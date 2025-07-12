using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class GetFeedbackSummaryQueryHandler : IRequestHandler<GetFeedbackSummaryQuery, FeedbackSummaryDTO>
    {
        private readonly IFeedbackService _feedbackService;

        public GetFeedbackSummaryQueryHandler(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public async Task<FeedbackSummaryDTO> Handle(GetFeedbackSummaryQuery request, CancellationToken cancellationToken)
        {
            return await _feedbackService.GetFeedbackSummaryByClassAsync(request.ClassID);
        }
    }
}
