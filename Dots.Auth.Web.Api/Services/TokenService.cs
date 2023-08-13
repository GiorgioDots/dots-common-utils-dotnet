using Dots.Auth.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Dots.Auth.Web.Api.Services
{
    public class TokenService
    {
        private readonly int ExpirationMinutes;
        private readonly IConfiguration config;

        public TokenService(IConfiguration config)
        {
            ExpirationMinutes = config.GetSection("JWT:tokenExpiration").Get<int?>() ?? 60 * 8;
            this.config = config;
        }

        public string CreateToken(Accounts account)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                new Claim("id", account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Email, account.Email)
            };

            var audiences = account.AccountsApplications
                .Select(k => new Claim(JwtRegisteredClaimNames.Aud, k.IdApplicationNavigation.Audience))
                .ToList();
            claims.AddRange(audiences);

            JwtSecurityToken token = new JwtSecurityToken(
                config.GetSection("JWT:defaultIssuer").Get<string>(),
                expires: expiration,
                signingCredentials: CreateSigningCredentials(),
                claims: claims
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        internal string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);

        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config.GetSection("JWT:secret").Get<string>() ?? "")
                ),
                SecurityAlgorithms.HmacSha256
            );
        }

    }
}
