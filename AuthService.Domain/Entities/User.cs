using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class User:BaseEntity
    {
        public string? Username { get; set; }           // نمایش به کاربر
        public byte[] RowVersion { get; set; } = default!;         // کنترل همزمانی (Concurrency Token)

        public ICollection<UserContact> Contacts { get; set; } = new List<UserContact>();
        public UserCredential? Credential { get; set; }
        public ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();

    }
}
