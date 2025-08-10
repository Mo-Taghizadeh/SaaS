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
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.Command
{
    public sealed record RegisterUserCommand(RegisterRequest Request) : IRequest<BaseResult_VM<RegisterResponse>>
    {

    }

    public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, BaseResult_VM<RegisterResponse>>
    {
        private readonly IAuthDbContext _db;
        private readonly IPasswordHasher _hasher;

        public RegisterUserHandler(IAuthDbContext db, IPasswordHasher hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task<BaseResult_VM<RegisterResponse>> Handle(RegisterUserCommand cmd, CancellationToken ct)
        {
            var r = cmd.Request;
            if (string.IsNullOrWhiteSpace(r.Email) || string.IsNullOrWhiteSpace(r.Username) || string.IsNullOrWhiteSpace(r.Password))
                return BaseResult_VM<RegisterResponse>.Fail((int)ErrorCodes.ValidationFailed, "Email, Username and Password are required.");

            var nEmail = Normalizer.NormalizeEmail(r.Email);
            var nUser = Normalizer.NormalizeUsername(r.Username);

            if (await _db.Users.AnyAsync(x => x.NormalizedEmail == nEmail, ct))
                return BaseResult_VM<RegisterResponse>.Fail((int)ErrorCodes.DuplicateEmail, "Email already exists.");

            if (await _db.Users.AnyAsync(x => x.NormalizedUsername == nUser, ct))
                return BaseResult_VM<RegisterResponse>.Fail((int)ErrorCodes.DuplicateUsername, "Username already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = r.Email.Trim(),
                NormalizedEmail = nEmail,
                Username = r.Username.Trim(),
                NormalizedUsername = nUser,
                PasswordHash = _hasher.Hash(r.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);

            return BaseResult_VM<RegisterResponse>.Ok(new RegisterResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email
            });
        }
    }
}
