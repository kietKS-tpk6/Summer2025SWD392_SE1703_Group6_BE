using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IWritingBaremRepository
    {
        Task AddRangeAsync(List<WritingBarem> barems);
        Task<List<WritingBarem>> GetByQuestionIDAsync(string questionID);

    }
}
