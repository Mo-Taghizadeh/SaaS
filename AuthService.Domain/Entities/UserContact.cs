using AuthService.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class UserContact : BaseEntity
    {
        public Guid UserId { get; set; }

        public ContactType Type { get; set; }   // Email/Mobile
        public string Value { get; set; } = default!; // Email (raw) یا Mobile (E.164)

        // فقط برای Email مفید است؛ برای ایندکس یونیک
        public string? EmailLower { get; private set; }   // computed persisted (LOWER(Value))

        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }

        public User User { get; set; } = default!;
    }
}
