using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class UpdateTestStatusCommandHandler : IRequestHandler<UpdateTestStatusCommand, string>
    {
        private readonly ITestService _testService;

        public UpdateTestStatusCommandHandler(ITestService testService)
        {
            _testService = testService;
        }

        public async Task<string> Handle(UpdateTestStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _testService.UpdateTestStatusAsync(request);

            if (result.Success)
                return result.Message;

            throw new ArgumentException(result.Message);
        }
    }
}