using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.Interfaces
{
    public interface IAuthDbContext
    {
        DbSet<User> Users { get; }
        DbSet<UserContact> UserContacts { get; }
        DbSet<UserCredential> UserCredentials { get; }
        DbSet<ExternalLogin> ExternalLogins { get; }
        DbSet<VerificationToken> VerificationTokens { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<OneTimeToken> OneTimeTokens { get; } 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
