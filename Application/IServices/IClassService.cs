using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Usecases.Command;

namespace Application.IServices
{
    public interface IClassService
    {
        Task<bool> CreateClassAsync(ClassCreateCommand  request);
        


    }
}
