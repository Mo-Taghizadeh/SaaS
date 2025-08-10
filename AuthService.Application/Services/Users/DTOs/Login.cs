using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.DTOs
{
    public sealed class LoginRequest
    {
        public string EmailOrUsername { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
        public DateTime ExpiresAtUtc { get; init; }
    }
}
