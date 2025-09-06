using AuthService.Application.Common.Interfaces;
using AuthService.Application.Common.Utils;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Entities.Enums;
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
            // پیدا کردن یوزر از طریق Contact
            var contact = await _db.UserContacts
                .Include(c => c.User).ThenInclude(u => u.Credential)
                .Where(c => (c.Type == ContactType.Email && c.EmailLower == cmd.Email.ToLower())
                         || (c.Type == ContactType.Mobile && c.Value == NormalizeMobile(cmd.Mobile)))
                .SingleOrDefaultAsync(ct);

            if (contact == null || contact.User.Status != UserStatus.Active)
                return BaseResult_VM.Failure("حساب کاربری یافت نشد یا غیرفعال است.");

            var user = contact.User;

            // بررسی پسورد
            if (!_passwordHasher.Verify(user.Credential.PasswordHash, cmd.Password))
                return BaseResult_VM.Failure("رمز عبور نادرست است.");

            // تولید JWT
            var token = _jwtService.Generate(user.Id, user.Username, ...);

            var rawRt = Crypto.NewSecureToken();
            var hashRt = Crypto.Sha256(rawRt);

            await _db.RefreshTokens.AddAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = hashRt,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByIp = null, // می‌تونی از req بیاری
                ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
            }, ct);

            await _db.SaveChangesAsync(ct);

            return BaseResult_VM<LoginResponse>.Ok(new LoginResponse
            {
                AccessToken = token,
                ExpiresAtUtc = expires,
                RefreshToken = rawRt
            });
        }
    }
}
