using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly HangulLearningSystemDbContext _dbContext;

        public OTPRepository(HangulLearningSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OTP> getOTPViaEmailAndCodeAsync(string email, string otpCode)
        {
            var otpEntity = await _dbContext.OTP
                .FirstOrDefaultAsync(x => x.Email == email && x.OTPCode == otpCode);

            return otpEntity; // trả về null nếu không tìm thấy
        }


        public async Task<bool> createOTP(OTP otp)
        {
            await _dbContext.OTP.AddAsync(otp);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateOTPViaOTPCodeAsync(string otpCode)
        {        
            var otpEntity = await _dbContext.OTP.FirstOrDefaultAsync(o => o.OTPCode == otpCode);

            if (otpEntity == null)
            {
                return false; 
            }         
            otpEntity.IsUsed = true;
  
            await _dbContext.SaveChangesAsync();

            return true;
        }


    }
}
