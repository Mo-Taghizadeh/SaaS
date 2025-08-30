using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }                   // PK
        public string Username { get; set; }           // نمایش به کاربر
        public string NormalizedUsername { get; set; } // برای Unique Index (CASE-INSENSITIVE)
        public string Email { get; set; }              // نمایش/ایمیل واقعی
        public string Mobile { get; set; }             // شماره تلفن همراه
        public string NormalizedEmail { get; set; }    // برای Unique Index
        public string PasswordHash { get; set; }       // هش (به همراه salt داخل هش)
        public bool IsActive { get; set; }             // فعال/غیرفعال
        public DateTime CreatedAt { get; set; }        // تاریخ ایجاد (UTC)
        public byte[] RowVersion { get; set; }         // کنترل همزمانی (Concurrency Token)
        public DateTime ModifiedAt { get; set; }
    }
}
