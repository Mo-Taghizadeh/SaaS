using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Utils
{
    public static class Crypto
    {
        public static string Sha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes); // طول 64 بایت هگز = 128 کاراکتر
        }

        public static string NewSecureToken(int bytesLen = 64)
        {
            var bytes = RandomNumberGenerator.GetBytes(bytesLen);
            return Convert.ToBase64String(bytes);
        }
    }
}
