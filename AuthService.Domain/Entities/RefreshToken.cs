using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // هش ذخیره می‌کنیم (نه متن خام)
        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedByIp { get; set; }

        public DateTime? RevokedAtUtc { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByTokenHash { get; set; } // برای rotation chain
        public bool IsRevoked => RevokedAtUtc.HasValue;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    }
}
