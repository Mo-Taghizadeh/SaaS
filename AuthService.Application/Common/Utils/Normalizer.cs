using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Utils
{
    public static class Normalizer
    {
        public static string NormalizeEmail(string email)
            => email?.Trim().ToUpperInvariant();

        public static string NormalizeUsername(string username)
            => username?.Trim().ToUpperInvariant();
    }
}
