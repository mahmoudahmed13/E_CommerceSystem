using E_Commerce.Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class TokenService : ITokenService
    {
        private readonly JwtSetting _jwtSetting;

        public TokenService(IOptions<JwtSetting> jwtSetting)
        {
            _jwtSetting = jwtSetting.Value;
        }
        public string CreateToken(string userId, string email, string userName, IReadOnlyList<string> roles)
        {
            //new JwtSecurityTokenHandler().WriteToken();

            // Claims
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName)
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            //Sensitive data => Add them in appsetting

            //SigningCredentials [Secret Key , Security Algo]

            var secretKey = _jwtSetting.SecretKey;
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("Jwt SecretKey Is Missing");
            if (secretKey.Length < 32)
                throw new InvalidOperationException("JWT SecretKey Is Too Short");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // issure + audience + expires

            var token = new JwtSecurityToken(
                issuer:_jwtSetting.Issuer,
                audience:_jwtSetting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSetting.ExpirationMinutes),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class JwtSetting
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpirationMinutes { get; set; }
    }
}
