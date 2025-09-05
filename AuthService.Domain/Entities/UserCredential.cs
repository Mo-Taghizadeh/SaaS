using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class UserCredential: BaseEntity
    {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; } = default!;
        public DateTime PasswordUpdatedAt { get; set; }
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

        public User User { get; set; } = default!;
    }
}
