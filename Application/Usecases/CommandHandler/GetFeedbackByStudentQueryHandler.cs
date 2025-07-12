using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class GetFeedbackByStudentQueryHandler : IRequestHandler<GetFeedbackByStudentQuery, List<FeedbackDTO>>
    {
        private readonly IFeedbackService _feedbackService;

        public GetFeedbackByStudentQueryHandler(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public async Task<List<FeedbackDTO>> Handle(GetFeedbackByStudentQuery request, CancellationToken cancellationToken)
        {
            return await _feedbackService.GetFeedbacksByStudentAsync(request.StudentID);
        }
    }
}
