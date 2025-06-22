using MediatR;

namespace Application.Usecases.Command
{
    public class DeleteTestSectionCommand : IRequest<string>
    {
        public string TestSectionID { get; set; }
        public string RequestingAccountID { get; set; }
    }
}