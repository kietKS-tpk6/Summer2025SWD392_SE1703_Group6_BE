using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class DeleteTestSectionCommandHandler : IRequestHandler<DeleteTestSectionCommand, string>
    {
        private readonly ITestSectionService _testSectionService;

        public DeleteTestSectionCommandHandler(ITestSectionService testSectionService)
        {
            _testSectionService = testSectionService;
        }

        public async Task<string> Handle(DeleteTestSectionCommand request, CancellationToken cancellationToken)
        {
            var result = await _testSectionService.DeleteTestSectionAsync(request);

            if (result.Success)
                return result.Message;

            throw new ArgumentException(result.Message);
        }
    }
}