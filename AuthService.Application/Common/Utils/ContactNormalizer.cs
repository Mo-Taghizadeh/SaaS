using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Utils
{
    public static class ContactNormalizer
    {
        /// <summary>
        /// نرمال‌سازی شماره موبایل به فرمت E.164 (مثلاً ایران: +98)
        /// </summary>
        public static string ToE164(string mobile)
        {
            var digits = new string(mobile.Where(char.IsDigit).ToArray());
            if (digits.StartsWith("00"))
                digits = digits[2..];
            if (digits.StartsWith("0"))
                digits = "98" + digits[1..];

            if (!digits.StartsWith("98") && !digits.StartsWith("+"))
                return "+" + digits;

            return digits.StartsWith("+") ? digits : "+" + digits;
        }
    }
}
