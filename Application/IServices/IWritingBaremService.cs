using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;

namespace Application.IServices
{
    public interface IWritingBaremService
    {
        Task<OperationResult<bool>> ValidateCreateBaremsAsync(List<CreateWritingBaremDTO> barems);
        Task<OperationResult<bool>> CreateWritingBaremsAsync(List<CreateWritingBaremDTO> barems);
    }

}
