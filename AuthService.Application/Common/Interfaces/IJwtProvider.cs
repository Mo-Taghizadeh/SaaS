using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(IEnumerable<Claim> claims, DateTime expiresUtc);
        TimeSpan AccessTokenLifetime { get; }
    }
}
