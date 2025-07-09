using MediatR;

namespace Application.Usecases.Command
{
    public class GetFeedbackByClassQuery : IRequest<List<FeedbackDTO>>
    {
        public string ClassID { get; set; }
    }
}
