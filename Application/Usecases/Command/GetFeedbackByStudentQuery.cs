using MediatR;

namespace Application.Usecases.Command
{
    public class GetFeedbackByStudentQuery : IRequest<List<FeedbackDTO>>
    {
        public string StudentID { get; set; }
    }
}
