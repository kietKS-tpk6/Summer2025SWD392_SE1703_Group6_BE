using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class GetFeedbackByIdQueryHandler : IRequestHandler<GetFeedbackByIdQuery, FeedbackDTO>
    {
        private readonly IFeedbackService _feedbackService;

        public GetFeedbackByIdQueryHandler(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public async Task<FeedbackDTO> Handle(GetFeedbackByIdQuery request, CancellationToken cancellationToken)
        {
            return await _feedbackService.GetFeedbackByIdAsync(request.FeedbackID);
        }
    }
}
