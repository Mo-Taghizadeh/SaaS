using AuthService.Application.Common.Interfaces;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

        public bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
