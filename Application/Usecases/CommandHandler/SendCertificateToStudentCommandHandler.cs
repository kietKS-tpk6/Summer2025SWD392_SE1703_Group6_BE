using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class SendCertificateToStudentCommandHandler : IRequestHandler<SendCertificateToStudentCommand, OperationResult<bool>>
    {
        private readonly ICertificateService _certificateService;

        public SendCertificateToStudentCommandHandler(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        public async Task<OperationResult<bool>> Handle(SendCertificateToStudentCommand request, CancellationToken cancellationToken)
        {        
            return await _certificateService.SendCertificateToStudentByStudentIDAndSubjectIDAsync(request.StudentID, request.ClassID);
        }

    }
}
