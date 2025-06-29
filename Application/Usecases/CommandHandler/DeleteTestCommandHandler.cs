using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class DeleteTestCommandHandler : IRequestHandler<DeleteTestCommand, string>
    {
        private readonly ITestService _testService;

        public DeleteTestCommandHandler(ITestService testService)
        {
            _testService = testService;
        }

        public async Task<string> Handle(DeleteTestCommand request, CancellationToken cancellationToken)
        {
            var result = await _testService.DeleteTestAsync(request);

            if (result.Success)
                return result.Message;

            throw new ArgumentException(result.Message);
        }
    }
}