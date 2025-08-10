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
    public sealed record SendEmailVerificationCommand(SendEmailVerificationRequest Request) : IRequest<BaseResult_VM>
    {
    }

    public sealed class SendEmailVerificationHandler
        : IRequestHandler<SendEmailVerificationCommand, BaseResult_VM>
    {
        private readonly IAuthDbContext _db;
        private readonly IEmailSender _email;

        public SendEmailVerificationHandler(IAuthDbContext db, IEmailSender email)
        {
            _db = db; _email = email;
        }

        public async Task<BaseResult_VM> Handle(SendEmailVerificationCommand cmd, CancellationToken ct)
        {
            var r = cmd.Request;
            if (r.UserId == Guid.Empty || string.IsNullOrWhiteSpace(r.Email))
                return BaseResult_VM.Fail(1001, "UserId and Email are required.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == r.UserId, ct);
            if (user is null || !user.IsActive || !string.Equals(user.Email, r.Email.Trim(), StringComparison.OrdinalIgnoreCase))
                return BaseResult_VM.Fail(1404, "User not found.");

            // ساخت توکن
            var raw = Crypto.NewSecureToken(48);
            var hash = Crypto.Sha256(raw);

            // می‌تونی قبلی‌های مصرف‌نشده را revoke کنی (اختیاری)

            await _db.OneTimeTokens.AddAsync(new OneTimeToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = hash,
                Purpose = TokenPurpose.EmailVerification,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(24)
            }, ct);
            await _db.SaveChangesAsync(ct);

            var link = r.CallbackUrl is null
                ? $"https://your-frontend/verify-email?uid={user.Id}&token={Uri.EscapeDataString(raw)}"
                : $"{r.CallbackUrl}?uid={user.Id}&token={Uri.EscapeDataString(raw)}";

            var subject = "Verify your email";
            var body = $"<p>Hello {user.Username},</p><p>Please verify your email by clicking <a href=\"{link}\">this link</a>.</p>";

            await _email.SendAsync(user.Email, subject, body, ct);

            return BaseResult_VM.Ok(message: "Verification email sent.");
        }
    }

}
