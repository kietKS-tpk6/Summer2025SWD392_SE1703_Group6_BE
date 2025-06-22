using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class CreateTestCommandHandler : IRequestHandler<CreateTestCommand, string>
    {
        private readonly ITestService _testService;

        public CreateTestCommandHandler(ITestService testService)
        {
            _testService = testService;
        }

        public async Task<string> Handle(CreateTestCommand request, CancellationToken cancellationToken)
        {
            var result = await _testService.CreateTestAsync(request);

            if (result.Success)
                return result.Data;

            throw new ArgumentException(result.Message);
        }
    }
}