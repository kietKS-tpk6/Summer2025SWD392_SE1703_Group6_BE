using MediatR;

namespace Application.Usecases.Command
{
    public class CreateEnrollmentCommand : IRequest<string>
    {
        public string PaymentID { get; set; }
        public string StudentID { get; set; }
        public string ClassID { get; set; }
    }
}