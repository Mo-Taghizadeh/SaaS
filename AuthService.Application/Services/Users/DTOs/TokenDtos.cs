using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Users.DTOs
{
    public sealed class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = default!;
        public string? Ip { get; set; }
    }

    public sealed class RefreshTokenResponse
    {
        public string AccessToken { get; init; } = default!;
        public DateTime ExpiresAtUtc { get; init; }
        public string NewRefreshToken { get; init; } = default!;
    }

    public sealed class LogoutRequest
    {
        public string RefreshToken { get; set; } = default!;
        public string? Ip { get; set; }
    }
}
