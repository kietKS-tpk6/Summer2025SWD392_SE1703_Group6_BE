using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Infrastructure.IRepositories;

namespace Infrastructure.Services
{
    public class  MCQAnswerDetailService : IMCQAnswerDetailService
    {
        private readonly IMCQAnswerDetailRepository _mCQAnswerDetailRepository;
        public MCQAnswerDetailService(IMCQAnswerDetailRepository mCQAnswerDetailRepository)
        {
            _mCQAnswerDetailRepository = mCQAnswerDetailRepository;
        }
    }
}
