using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.DTOs
{
    public sealed class SendEmailVerificationRequest
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = default!;
        public string? CallbackUrl { get; set; } // مثلا لینک فرانت
    }

    public sealed class VerifyEmailRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = default!;
    }
}
