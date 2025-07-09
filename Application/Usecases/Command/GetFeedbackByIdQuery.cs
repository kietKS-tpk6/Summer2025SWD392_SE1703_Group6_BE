using Application.DTOs;
using MediatR;

namespace Application.Usecases.Command
{
    public class GetFeedbackByIdQuery : IRequest<FeedbackDTO>
    {
        public string FeedbackID { get; set; }
    }
}
