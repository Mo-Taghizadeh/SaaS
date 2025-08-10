using AuthService.Application.Common.Interfaces;
using AuthService.Application.Common.Utils;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.DTOs;
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
    public sealed record LoginUserQuery(LoginRequest Request) : IRequest<BaseResult_VM<LoginResponse>>
    {

    }

    public sealed class LoginUserHandler : IRequestHandler<LoginUserQuery, BaseResult_VM<LoginResponse>>
    {
        private readonly IAuthDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtProvider _jwt;

        public LoginUserHandler(IAuthDbContext db, IPasswordHasher hasher, IJwtProvider jwt)
        {
            _db = db;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<BaseResult_VM<LoginResponse>> Handle(LoginUserQuery cmd, CancellationToken ct)
        {
            var r = cmd.Request;
            if (string.IsNullOrWhiteSpace(r.EmailOrUsername) || string.IsNullOrWhiteSpace(r.Password))
                return BaseResult_VM<LoginResponse>.Fail((int)ErrorCodes.ValidationFailed, "Credentials required.");

            var key = r.EmailOrUsername.Trim();
            var nEmail = Normalizer.NormalizeEmail(key);
            var nUser = Normalizer.NormalizeUsername(key);

            var user = await _db.Users.FirstOrDefaultAsync(
                x => x.NormalizedEmail == nEmail || x.NormalizedUsername == nUser, ct);

            if (user is null || !_hasher.Verify(r.Password, user.PasswordHash) || !user.IsActive)
                return BaseResult_VM<LoginResponse>.Fail((int)ErrorCodes.Unauthorized, "Invalid credentials.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username ?? ""),
                new(ClaimTypes.Email, user.Email ?? ""),
                new(ClaimTypes.Role, "User")
            };

            var expires = DateTime.UtcNow.Add(_jwt.AccessTokenLifetime);
            var token = _jwt.GenerateToken(claims, expires);

            return BaseResult_VM<LoginResponse>.Ok(new LoginResponse
            {
                AccessToken = token,
                ExpiresAtUtc = expires
            });
        }
    }
}
