using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class MCQAnswerService : IMCQAnswerService
    {
        private readonly IMCQAnswerRepository _mCQAnswerRepository;
        public MCQAnswerService(IMCQAnswerRepository mCQAnswerRepository)
        {
            _mCQAnswerRepository = mCQAnswerRepository;
        }
    }
}
