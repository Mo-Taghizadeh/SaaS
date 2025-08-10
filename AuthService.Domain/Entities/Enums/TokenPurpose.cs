using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities.Enums
{
    public enum TokenPurpose
    {
        EmailVerification = 1,
        PasswordReset = 2
    }
}
