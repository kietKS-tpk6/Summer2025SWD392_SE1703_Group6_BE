using MediatR;

namespace Application.Usecases.Command
{
    public class CreateFeedbackCommand : IRequest<string>
    {
        public string ClassID { get; set; }
        public string StudentID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}