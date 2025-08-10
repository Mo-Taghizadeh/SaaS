using AuthService.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.JWT
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _cfg;
        private readonly TimeSpan _lifetime;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly SymmetricSecurityKey _key;

        public JwtProvider(IConfiguration cfg)
        {
            _cfg = cfg;
            _issuer = _cfg["Jwt:Issuer"];
            _audience = _cfg["Jwt:Audience"];
            _lifetime = TimeSpan.FromMinutes(int.Parse(_cfg["Jwt:LifetimeMinutes"] ?? "60"));
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        }

        public TimeSpan AccessTokenLifetime => _lifetime;

        public string GenerateToken(IEnumerable<Claim> claims, DateTime expiresUtc)
        {
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresUtc,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
