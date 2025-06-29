using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class UpdateTestCommandHandler : IRequestHandler<UpdateTestCommand, string>
    {
        private readonly ITestService _testService;

        public UpdateTestCommandHandler(ITestService testService)
        {
            _testService = testService;
        }

        public async Task<string> Handle(UpdateTestCommand request, CancellationToken cancellationToken)
        {
            var result = await _testService.UpdateTestAsync(request);

            if (result.Success)
                return result.Message;

            throw new ArgumentException(result.Message);
        }
    }
}