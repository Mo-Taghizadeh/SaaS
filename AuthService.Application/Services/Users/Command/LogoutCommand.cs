using AuthService.Application.Common.Interfaces;
using AuthService.Application.Common.Utils;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.Command
{
    public sealed record LogoutCommand(LogoutRequest Request) : IRequest<BaseResult_VM>
    {
    }

    public sealed class LogoutHandler : IRequestHandler<LogoutCommand, BaseResult_VM>
    {
        private readonly IAuthDbContext _db;
        public LogoutHandler(IAuthDbContext db) => _db = db;

        public async Task<BaseResult_VM> Handle(LogoutCommand cmd, CancellationToken ct)
        {
            var req = cmd.Request;
            if (string.IsNullOrWhiteSpace(req.RefreshToken))
                return BaseResult_VM.Fail(1001, "Refresh token required.");

            var hash = Crypto.Sha256(req.RefreshToken);

            var rt = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (rt is null)
                return BaseResult_VM.Ok(message: "Already logged out."); // idempotent

            if (!rt.IsRevoked)
            {
                rt.RevokedAtUtc = DateTime.UtcNow;
                rt.RevokedByIp = req.Ip;
                await _db.SaveChangesAsync(ct);
            }

            return BaseResult_VM.Ok(message: "Logged out.");
        }
    }
}
