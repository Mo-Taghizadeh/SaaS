using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class ExternalLogin : BaseEntity
    {
        public Guid UserId { get; set; }

        public string Provider { get; set; } = default!;       // "google"
        public string ProviderUserId { get; set; } = default!; // sub از Google
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = default!;
    }
}
