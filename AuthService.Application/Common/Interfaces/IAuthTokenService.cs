using AuthService.Application.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Interfaces
{
    public interface IAuthTokenService
    {
        Task<AuthTokens_VM> IssueTokens(Guid userId, string username);
    }
}
