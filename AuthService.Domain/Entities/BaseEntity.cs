using AuthService.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }                   // PK
        public UserStatus Status { get; set; } = UserStatus.Active;             // وضعیت
        public DateTime CreatedAt { get; set; }        // تاریخ ایجاد (UTC)
        public DateTime ModifiedAt { get; set; }
    }
}
