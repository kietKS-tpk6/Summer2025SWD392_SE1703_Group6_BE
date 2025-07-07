using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.DTOs;

namespace Application.IServices
{
    public interface IDashboardManagerService
    {
        Task<OperationResult<ManagerSidebarRightDTO>> GetDataForSidebarRightAsync();
    }
}
