using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.DTOs
{
    public sealed class ForgotPasswordRequest
    {
        public string Email { get; set; } = default!;
        public string? CallbackUrl { get; set; }
    }

    public sealed class ResetPasswordRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }

    public sealed class ChangePasswordRequest
    {
        public Guid UserId { get; set; }            // یا از Claims بگیر
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
