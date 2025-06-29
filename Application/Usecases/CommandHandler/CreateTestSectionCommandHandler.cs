using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class CreateTestSectionCommandHandler : IRequestHandler<CreateTestSectionCommand, string>
    {
        private readonly ITestSectionService _testSectionService;

        public CreateTestSectionCommandHandler(ITestSectionService testSectionService)
        {
            _testSectionService = testSectionService;
        }

        public async Task<string> Handle(CreateTestSectionCommand request, CancellationToken cancellationToken)
        {
            var result = await _testSectionService.CreateTestSectionAsync(request);

            if (result.Success)
                return result.Data;

            throw new ArgumentException(result.Message);
        }
    }
}