using AuthService.Application.Common.Interfaces;
using AuthService.Application.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Security
{
    public sealed class JwtTokenService : IAuthTokenService
    {
        private readonly IJwtProvider _provider;

        public JwtTokenService(IJwtProvider provider)
        {
            _provider = provider;
        }

        //public AuthTokens_VM IssueTokens(Guid userId, string username)
        //{
        //    // Claims اصلی
        //    var claims = new List<Claim>
        //{
        //    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        //    new Claim(JwtRegisteredClaimNames.UniqueName, username),
        //    new Claim("uid", userId.ToString()),
        //    new Claim("uname", username),
        //};

        //    // Access Token
        //    var accessExpires = DateTime.UtcNow.Add(_provider.AccessTokenLifetime);
        //    var accessToken = _provider.GenerateToken(claims, accessExpires);

        //    // Refresh Token (می‌تونی ساده بسازی یا هم با JWT)
        //    var refreshExpires = DateTime.UtcNow.AddDays(7); // به‌دلخواه
        //    var refreshToken = Guid.NewGuid().ToString("N"); // یا میشه JWT جدا

        //    return new AuthTokens_VM
        //    {
        //        AccessToken = accessToken,
        //        RefreshToken = refreshToken,
        //        AccessTokenExpiresAt = accessExpires,
        //        RefreshTokenExpiresAt = refreshExpires,
        //        UserId = userId,
        //        Username = username
        //    };
        //}

        public Task<AuthTokens_VM> IssueTokens(Guid userId, string username)
        {
            var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username ?? string.Empty),
            new Claim("uid", userId.ToString()),
            new Claim("uname", username ?? string.Empty)
        };

            var accessExpires = DateTime.UtcNow.Add(_provider.AccessTokenLifetime);
            var access = _provider.GenerateToken(claims, accessExpires);

            var refreshExpires = DateTime.UtcNow.AddDays(7);
            var refresh = Guid.NewGuid().ToString("N");

            return Task.FromResult(new AuthTokens_VM
            {
                AccessToken = access,
                RefreshToken = refresh,
                AccessTokenExpiresAt = accessExpires,
                RefreshTokenExpiresAt = refreshExpires,
                UserId = userId,
                Username = username ?? string.Empty
            });
        }
    }
}
