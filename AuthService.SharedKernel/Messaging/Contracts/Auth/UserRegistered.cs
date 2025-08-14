using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.SharedKernel.Messaging.Contracts.Auth
{
    // رویداد وقتی کاربر ثبت‌نام می‌کند
    public sealed record UserRegistered(Guid UserId, string Email, string Username, DateTime OccurredAtUtc);
}
