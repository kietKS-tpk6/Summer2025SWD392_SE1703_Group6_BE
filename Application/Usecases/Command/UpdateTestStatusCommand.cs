using MediatR;
using Domain.Enums;

namespace Application.Usecases.Command
{
    public class UpdateTestStatusCommand : IRequest<string>
    {
        public string TestID { get; set; }
        public TestStatus NewStatus { get; set; }
        public string RequestingAccountID { get; set; }
    }
}