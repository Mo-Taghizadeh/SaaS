using AuthService.Application.Common.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Context
{
    public class AuthDbContext : DbContext , IAuthDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OneTimeToken> OneTimeTokens => Set<OneTimeToken>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // اعمال همه کانفیگ‌های IEntityTypeConfiguration موجود در اسمبلی Infrastructure
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

    }
}
