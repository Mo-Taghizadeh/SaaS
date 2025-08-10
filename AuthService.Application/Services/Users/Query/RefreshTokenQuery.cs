using AuthService.Application.Common.Interfaces;
using AuthService.Application.Common.Utils;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.DTOs;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.Query
{
    public sealed record RefreshTokenQuery(RefreshTokenRequest Request) : IRequest<BaseResult_VM<RefreshTokenResponse>>
    {

    }

    public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenQuery, BaseResult_VM<RefreshTokenResponse>>
    {
        private readonly IAuthDbContext _db;
        private readonly IJwtProvider _jwt;

        public RefreshTokenHandler(IAuthDbContext db, IJwtProvider jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<BaseResult_VM<RefreshTokenResponse>> Handle(RefreshTokenQuery cmd, CancellationToken ct)
        {
            var req = cmd.Request;
            if (string.IsNullOrWhiteSpace(req.RefreshToken))
                return BaseResult_VM<RefreshTokenResponse>.Fail(1001, "Refresh token required.");

            var hash = Crypto.Sha256(req.RefreshToken);

            var rt = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (rt is null || rt.IsRevoked || rt.IsExpired)
                return BaseResult_VM<RefreshTokenResponse>.Fail(1401, "Invalid refresh token.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == rt.UserId, ct);
            if (user is null || !user.IsActive)
                return BaseResult_VM<RefreshTokenResponse>.Fail(1401, "User not allowed.");

            // rotate: revoke old, issue new
            var newRaw = Crypto.NewSecureToken();
            var newHash = Crypto.Sha256(newRaw);

            rt.RevokedAtUtc = DateTime.UtcNow;
            rt.RevokedByIp = req.Ip;
            rt.ReplacedByTokenHash = newHash;

            var newRt = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newHash,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByIp = req.Ip,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(30) // نگهداری در config هم میشه
            };

            await _db.RefreshTokens.AddAsync(newRt, ct);

            // new access token
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username ?? ""),
                new(ClaimTypes.Email, user.Email ?? ""),
                new(ClaimTypes.Role, "User")
            };
            var expires = DateTime.UtcNow.Add(_jwt.AccessTokenLifetime);
            var access = _jwt.GenerateToken(claims, expires);

            await _db.SaveChangesAsync(ct);

            return BaseResult_VM<RefreshTokenResponse>.Ok(new RefreshTokenResponse
            {
                AccessToken = access,
                ExpiresAtUtc = expires,
                NewRefreshToken = newRaw
            });
        }
    }

}
