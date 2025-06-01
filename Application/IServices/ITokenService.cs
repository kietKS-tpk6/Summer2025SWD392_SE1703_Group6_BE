using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims, int expirationHours);
        IEnumerable<Claim> ValidateToken(string token);
        bool IsTokenValid(string token);
        string GetClaimFromToken(string token, string claimType);
    }
}
