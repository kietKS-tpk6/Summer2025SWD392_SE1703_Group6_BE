using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class WritingAnswerService : IWritingAnswerService
    {
        private readonly IWritingAnswerRepository _writingAnswerRepository;
        public WritingAnswerService(IWritingAnswerRepository writingAnswerRepository)
        {
            _writingAnswerRepository = writingAnswerRepository;
        }
    }
}
