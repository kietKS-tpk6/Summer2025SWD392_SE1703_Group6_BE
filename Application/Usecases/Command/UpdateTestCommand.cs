using MediatR;

namespace Application.Usecases.Command
{
    public class UpdateTestCommand : IRequest<string>
    {
        public string TestID { get; set; }
        public string TestName { get; set; }
        public string RequestingAccountID { get; set; }
    }
}