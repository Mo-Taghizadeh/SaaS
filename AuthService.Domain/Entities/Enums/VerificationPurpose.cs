using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities.Enums
{
    public enum VerificationPurpose : byte
    {
        VerifyEmail = 1, 
        VerifyMobile = 2, 
        ResetPassword = 3, 
        LoginOtp = 4
    }
}
