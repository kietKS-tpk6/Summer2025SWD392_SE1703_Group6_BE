using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class UpdateFeedbackCommandHandler : IRequestHandler<UpdateFeedbackCommand, bool>
    {
        private readonly IFeedbackService _feedbackService;

        public UpdateFeedbackCommandHandler(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public async Task<bool> Handle(UpdateFeedbackCommand request, CancellationToken cancellationToken)
        {
            return await _feedbackService.UpdateFeedbackAsync(request.FeedbackID, request.Rating, request.Comment);
        }
    }
}