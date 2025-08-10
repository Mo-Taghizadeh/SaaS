using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Context.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> b)
        {
            b.ToTable("RefreshTokens", "auth");
            b.HasKey(x => x.Id);

            b.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
            b.Property(x => x.CreatedAtUtc).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            b.Property(x => x.ExpiresAtUtc).HasColumnType("datetime2");

            b.HasIndex(x => new { x.UserId, x.TokenHash }).IsUnique();
            b.HasIndex(x => x.ExpiresAtUtc);
        }
    }
}
