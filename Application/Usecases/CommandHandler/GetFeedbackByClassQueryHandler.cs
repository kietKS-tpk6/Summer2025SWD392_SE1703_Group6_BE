using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class GetFeedbackByClassQueryHandler : IRequestHandler<GetFeedbackByClassQuery, List<FeedbackDTO>>
    {
        private readonly IFeedbackService _feedbackService;

        public GetFeedbackByClassQueryHandler(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public async Task<List<FeedbackDTO>> Handle(GetFeedbackByClassQuery request, CancellationToken cancellationToken)
        {
            return await _feedbackService.GetFeedbacksByClassAsync(request.ClassID);
        }
    }
}
