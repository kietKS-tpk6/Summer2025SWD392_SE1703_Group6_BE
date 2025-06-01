using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.IServices;
namespace Infrastructure.Services
{
    public class TokenService : ITokenService // Implement interface từ Application
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _key;

        public TokenService(IConfiguration configuration)
        {
            _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentException("Missing Jwt:Issuer");
            _audience = configuration["Jwt:Audience"] ?? throw new ArgumentException("Missing Jwt:Audience");

            var secretKey = configuration["Jwt:Key"] ?? throw new ArgumentException("Missing Jwt:Key");
            _key = Encoding.UTF8.GetBytes(secretKey);
        }

        public string GenerateToken(IEnumerable<Claim> claims, int expirationHours)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expirationHours),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IEnumerable<Claim> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return ((JwtSecurityToken)validatedToken).Claims;
        }

        public bool IsTokenValid(string token)
        {
            try
            {
                ValidateToken(token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetClaimFromToken(string token, string claimType)
        {
            try
            {
                var claims = ValidateToken(token);
                return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
