using AuthService.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class OneTimeToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // هش توکن (متن خام رو هرگز ذخیره نکن)
        public string TokenHash { get; set; } = default!;
        public TokenPurpose Purpose { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? ConsumedAtUtc { get; set; }

        public bool IsConsumed => ConsumedAtUtc.HasValue;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    }
}
