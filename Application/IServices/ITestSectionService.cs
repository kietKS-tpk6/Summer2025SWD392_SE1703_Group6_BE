using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Domain.Enums;

namespace Application.IServices
{
    public interface ITestSectionService
    {
        Task<bool> IsTestSectionExistAsync(string testSectionId);
        Task<OperationResult<string>> ValidateSectionTypeMatchFormatAsync(string testSectionId, TestFormatType formatType);

    }
}
