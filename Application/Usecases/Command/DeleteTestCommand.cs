using MediatR;

namespace Application.Usecases.Command
{
    public class DeleteTestCommand : IRequest<string>
    {
        public string TestID { get; set; }
        public string RequestingAccountID { get; set; }
    }
}