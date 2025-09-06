using AuthService.Application.Common.Interfaces;
using AuthService.Application.Common.Utils;
using AuthService.Application.Common.ViewModels;
using AuthService.Application.Services.Users.DTOs;
using AuthService.SharedKernel.Messaging.Contracts.Auth;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Domain.Entities.Enums;

namespace AuthService.Application.Services.Users.Command
{
    public sealed record RegisterUserCommand(RegisterRequest Request) : IRequest<BaseResult_VM<AuthTokens_VM>>
    {
        public string Identifier { get; init; } = default!; // ایمیل یا موبایل
        public string Password { get; init; } = default!;
    }

    public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, BaseResult_VM<AuthTokens_VM>>
    {
        private readonly IAuthDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IMessageBus _bus;
        private readonly IAuthTokenService _jwtTokenService;

        public RegisterUserHandler(IAuthDbContext db, IPasswordHasher hasher, IMessageBus bus)
        {
            _db = db;
            _hasher = hasher;
            _bus = bus;
        }

        public async Task<BaseResult_VM<AuthTokens_VM>> Handle(RegisterUserCommand cmd, CancellationToken ct)
        {
            var id = cmd.Identifier.Trim();
            bool looksEmail = id.Contains("@");

            var q = _db.UserContacts
                .Include(c => c.User)
                .Include(c => c.User.Credential)
                .AsQueryable();

            UserContact? contact;
            if (looksEmail)
            {
                var emailLower = id.ToLowerInvariant();
                contact = await q.SingleOrDefaultAsync(c => c.Type == ContactType.Email && c.EmailLower == emailLower, ct);
            }
            else
            {
                var mobile = ContactNormalizer.ToE164(id);
                contact = await q.SingleOrDefaultAsync(c => c.Type == ContactType.Mobile && c.Value == mobile, ct);
            }

            if (contact == null || contact.User.Status != UserStatus.Active)
                return BaseResult_VM<AuthTokens_VM>.Fail(401, "نام کاربری/ایمیل یا رمز عبور نادرست است.");

            if (!_hasher.Verify(contact.User.Credential!.PasswordHash, cmd.Password))
                return BaseResult_VM<AuthTokens_VM>.Fail(401, "نام کاربری/ایمیل یا رمز عبور نادرست است.");

            if (!contact.IsVerified)
                return BaseResult_VM<AuthTokens_VM>.Fail(403, "شناسه شما هنوز تایید نشده است.");

            var tokens = await _jwtTokenService.IssueTokens(contact.UserId,contact.User?.Username ?? contact.UserId.ToString());
            return BaseResult_VM<AuthTokens_VM>.Ok(tokens, "ورود موفق");
        }
    }
}
