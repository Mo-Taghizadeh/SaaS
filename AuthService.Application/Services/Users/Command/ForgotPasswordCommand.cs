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
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.Command
{
    public sealed record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<BaseResult_VM>
    {
    }

    public sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, BaseResult_VM>
    {
        private readonly IAuthDbContext _db;
        private readonly IEmailSender _email;

        public ForgotPasswordHandler(IAuthDbContext db, IEmailSender email) { _db = db; _email = email; }

        public async Task<BaseResult_VM> Handle(ForgotPasswordCommand cmd, CancellationToken ct)
        {
            var r = cmd.Request;
            if (string.IsNullOrWhiteSpace(r.Email))
                return BaseResult_VM.Fail(1001, "Email required.");

            var normalized = r.Email.Trim().ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normalized, ct);
            if (user is null || !user.IsActive)
                return BaseResult_VM.Ok(message: "If account exists, a reset email is sent."); // جلوگیری از افشای وجود/عدم‌وجود

            var raw = Crypto.NewSecureToken(48);
            var hash = Crypto.Sha256(raw);

            await _db.OneTimeTokens.AddAsync(new OneTimeToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = hash,
                Purpose = TokenPurpose.PasswordReset,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1)
            }, ct);
            await _db.SaveChangesAsync(ct);

            var link = r.CallbackUrl is null
                ? $"https://your-frontend/reset-password?uid={user.Id}&token={Uri.EscapeDataString(raw)}"
                : $"{r.CallbackUrl}?uid={user.Id}&token={Uri.EscapeDataString(raw)}";

            var subject = "Reset your password";
            var body = $"<p>Hello {user.Username},</p><p>Click <a href=\"{link}\">here</a> to reset your password. This link expires in 1 hour.</p>";

            await _email.SendAsync(user.Email, subject, body, ct);

            return BaseResult_VM.Ok(message: "If account exists, a reset email is sent.");
        }
    }


}
