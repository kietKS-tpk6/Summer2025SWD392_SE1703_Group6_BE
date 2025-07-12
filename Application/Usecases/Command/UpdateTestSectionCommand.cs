using MediatR;
using Domain.Enums;

namespace Application.Usecases.Command
{
    public class UpdateTestSectionCommand : IRequest<string>
    {
        public string TestSectionID { get; set; }
        public string? Context { get; set; }
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
        public decimal? Score { get; set; }
        public string RequestingAccountID { get; set; }
    }
}