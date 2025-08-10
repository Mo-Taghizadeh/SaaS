using AuthService.Application.Common.Interfaces;
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
    public sealed record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<BaseResult_VM>
    {
    }

    public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, BaseResult_VM>
    {
        private readonly IAuthDbContext _db;
        private readonly IPasswordHasher _hasher;

        public ChangePasswordHandler(IAuthDbContext db, IPasswordHasher hasher)
        { _db = db; _hasher = hasher; }

        public async Task<BaseResult_VM> Handle(ChangePasswordCommand cmd, CancellationToken ct)
        {
            var r = cmd.Request;
            if (r.UserId == Guid.Empty || string.IsNullOrWhiteSpace(r.CurrentPassword) || string.IsNullOrWhiteSpace(r.NewPassword))
                return BaseResult_VM.Fail(1001, "Invalid request.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == r.UserId, ct);
            if (user is null || !user.IsActive)
                return BaseResult_VM.Fail(1404, "User not found.");

            if (!_hasher.Verify(r.CurrentPassword, user.PasswordHash))
                return BaseResult_VM.Fail(1401, "Current password is incorrect.");

            user.PasswordHash = _hasher.Hash(r.NewPassword);
            await _db.SaveChangesAsync(ct);

            return BaseResult_VM.Ok(message: "Password changed.");
        }
    }
}
