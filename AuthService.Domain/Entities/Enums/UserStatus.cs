using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities.Enums
{
    public enum UserStatus : byte
    {
        Active = 1, 
        Suspended = 2, 
        Deleted = 3
    }
}
