using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, string>
    {
        private readonly IFeedbackService _feedbackService;

        public CreateFeedbackCommandHandler(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public async Task<string> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            return await _feedbackService.CreateFeedbackAsync(request);
        }
    }
}