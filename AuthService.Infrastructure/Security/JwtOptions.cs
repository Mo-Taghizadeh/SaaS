using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Security
{
    public sealed class JwtOptions
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string SigningKey { get; set; } = default!;
        public int AccessTokenMinutes { get; set; } = 60; // پیش‌فرض ۱ ساعت
    }
}
