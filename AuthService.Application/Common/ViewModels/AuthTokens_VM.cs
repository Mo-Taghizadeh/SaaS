using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.ViewModels
{
    public sealed class AuthTokens_VM
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
        public DateTime AccessTokenExpiresAt { get; init; }
        public DateTime RefreshTokenExpiresAt { get; init; }

        // اختیاری: برای فرانت
        public string TokenType { get; init; } = "Bearer";

        // اختیاری: متادیتا (مثلا DeviceId, UserId, Username)
        public Guid UserId { get; init; }
        public string Username { get; init; } = default!;
    }
}
