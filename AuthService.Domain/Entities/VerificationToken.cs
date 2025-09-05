using AuthService.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class VerificationToken: BaseEntity
    {
        public Guid? UserId { get; set; } // ممکن است قبل از ایجاد User باشد (ثبت‌نام با ایمیل/موبایل)

        public VerificationPurpose Purpose { get; set; }
        public ContactType ContactType { get; set; }
        public string ContactValue { get; set; } = default!;   // email یا mobile (E.164)

        public string TokenHash { get; set; } = default!;      // کد/توکن هش‌شده
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }

        public User? User { get; set; }
    }
}
