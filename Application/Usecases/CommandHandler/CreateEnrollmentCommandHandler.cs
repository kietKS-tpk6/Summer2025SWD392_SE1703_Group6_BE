using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Usecases.CommandHandler
{
    public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, string>
    {
        private readonly IEnrollmentService _enrollmentService;

        public CreateEnrollmentCommandHandler(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        public async Task<string> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            return await _enrollmentService.CreateEnrollmentAsync(request);
        }
    }
}