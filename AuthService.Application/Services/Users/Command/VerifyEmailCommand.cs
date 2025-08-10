using AuthService.Application.Common.Interfaces;
using AuthService.Application.Common.Utils;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.DTOs;
using AuthService.Domain.Entities.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.Command
{
    public sealed record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest<BaseResult_VM>
    {
    }

    public sealed class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, BaseResult_VM>
    {
        private readonly IAuthDbContext _db;

        public VerifyEmailHandler(IAuthDbContext db) => _db = db;

        public async Task<BaseResult_VM> Handle(VerifyEmailCommand cmd, CancellationToken ct)
        {
            var r = cmd.Request;
            if (r.UserId == Guid.Empty || string.IsNullOrWhiteSpace(r.Token))
                return BaseResult_VM.Fail(1001, "Invalid request.");

            var hash = Crypto.Sha256(r.Token);

            var token = await _db.OneTimeTokens.FirstOrDefaultAsync(
                x => x.UserId == r.UserId && x.Purpose == TokenPurpose.EmailVerification && x.TokenHash == hash, ct);

            if (token is null || token.IsExpired || token.IsConsumed)
                return BaseResult_VM.Fail(1401, "Invalid or expired token.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == r.UserId, ct);
            if (user is null || !user.IsActive)
                return BaseResult_VM.Fail(1404, "User not found.");

            // علامت‌گذاری مصرف
            token.ConsumedAtUtc = DateTime.UtcNow;

            // می‌تونی فیلد EmailVerifiedAt اضافه کنی؛ فعلاً فرض: IsActive کافی است
            // مثلا:
            // user.EmailVerifiedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return BaseResult_VM.Ok(message: "Email verified.");
        }
    }
}
