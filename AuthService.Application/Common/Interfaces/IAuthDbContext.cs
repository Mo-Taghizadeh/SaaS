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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
