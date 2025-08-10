using AuthService.Application.Common.Interfaces;
using AuthService.Application.Common.Utils;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.Command
{
    public sealed record ResetPasswordCommand(ResetPasswordRequest Request): IRequest<BaseResult_VM>
    {
    }

    public sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, BaseResult_VM>
    {
        private readonly IAuthDbContext _db;
        private readonly IPasswordHasher _hasher;

        public ResetPasswordHandler(IAuthDbContext db, IPasswordHasher hasher)
        { _db = db; _hasher = hasher; }

        public async Task<BaseResult_VM> Handle(ResetPasswordCommand cmd, CancellationToken ct)
        {
            var r = cmd.Request;
            if (r.UserId == Guid.Empty || string.IsNullOrWhiteSpace(r.Token) || string.IsNullOrWhiteSpace(r.NewPassword))
                return BaseResult_VM.Fail(1001, "Invalid request.");

            var hash = Crypto.Sha256(r.Token);

            var token = await _db.OneTimeTokens.FirstOrDefaultAsync(
                x => x.UserId == r.UserId && x.Purpose == TokenPurpose.PasswordReset && x.TokenHash == hash, ct);

            if (token is null || token.IsExpired || token.IsConsumed)
                return BaseResult_VM.Fail(1401, "Invalid or expired token.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == r.UserId, ct);
            if (user is null || !user.IsActive)
                return BaseResult_VM.Fail(1404, "User not found.");

            user.PasswordHash = _hasher.Hash(r.NewPassword);
            token.ConsumedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return BaseResult_VM.Ok(message: "Password has been reset.");
        }
    }
}
