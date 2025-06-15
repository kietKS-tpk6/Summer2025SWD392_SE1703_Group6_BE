using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.IRepositories
{
    public interface IOTPRepository
    {
        public Task<OTP> getOTPViaEmailAndCodeAsync(string email,string otp);
        public Task<bool> createOTP(OTP otp);

        public Task<bool> UpdateOTPViaOTPCodeAsync(string otp);

       


    }
}
