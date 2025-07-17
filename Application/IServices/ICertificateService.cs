using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
namespace Application.IServices
{
    public interface ICertificateService
    {
     //   Task<OperationResult<bool>> SendCertificateToStudentAsync(string email, string studentName, string subjectName);
        Task<OperationResult<bool>> SendCertificateToStudentByStudentIDAndSubjectIDAsync(string studentID, string ClassID);
        Task<OperationResult<List<CertificateSendErrorDTO>>> SendCertificatesToClassAsync(string classId);

    }
}
