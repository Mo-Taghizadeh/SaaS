using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Context.Configurations
{
    public class UserCredentialConfig : IEntityTypeConfiguration<UserCredential>
    {
        public void Configure(EntityTypeBuilder<UserCredential> b)
        {
            b.ToTable("UserCredentials", "auth");
            b.HasKey(x => x.UserId); // 1:1

            b.Property(x => x.PasswordHash)
                .HasMaxLength(512)
                .IsRequired();

            b.Property(x => x.PasswordUpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.SecurityStamp)
                .HasMaxLength(64)
                .IsRequired();

            b.HasIndex(x => x.SecurityStamp)
                .HasDatabaseName("IX_UserCredentials_SecurityStamp");
        }
    }
}
