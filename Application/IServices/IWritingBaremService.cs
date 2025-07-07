using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IWritingBaremService
    {
        Task<OperationResult<bool>> ValidateCreateBaremsAsync(List<CreateWritingBaremDTO> barems);
        Task<OperationResult<bool>> CreateWritingBaremsAsync(List<CreateWritingBaremDTO> barems);
        Task<OperationResult<List<WritingBaremDTO>>> GetByQuestionIDAsync(string questionID);
        Task<OperationResult<bool>> UpdateWritingBaremAsync(UpdateWritingBaremCommand command);
        Task<OperationResult<bool>> ValidateUpdateBaremAsync(UpdateWritingBaremCommand command);
        Task<OperationResult<bool>> SoftDeleteWritingBaremAsync(string writingBaremID);

    }

}
