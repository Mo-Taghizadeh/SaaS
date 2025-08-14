using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.SharedKernel.Messaging.Contracts.Auth
{
    public sealed record PasswordChanged(Guid UserId, DateTime OccurredAtUtc);
}
