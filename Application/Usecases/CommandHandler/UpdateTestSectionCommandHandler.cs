using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class UpdateTestSectionCommandHandler : IRequestHandler<UpdateTestSectionCommand, string>
    {
        private readonly ITestSectionService _testSectionService;

        public UpdateTestSectionCommandHandler(ITestSectionService testSectionService)
        {
            _testSectionService = testSectionService;
        }

        public async Task<string> Handle(UpdateTestSectionCommand request, CancellationToken cancellationToken)
        {
            var result = await _testSectionService.UpdateTestSectionAsync(request);

            if (result.Success)
                return result.Message;

            throw new ArgumentException(result.Message);
        }
    }
}