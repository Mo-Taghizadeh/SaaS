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
    public class ExternalLoginConfig : IEntityTypeConfiguration<ExternalLogin>
    {
        public void Configure(EntityTypeBuilder<ExternalLogin> b)
        {
            b.ToTable("ExternalLogins", "auth");
            b.HasKey(x => x.Id);

            b.Property(x => x.Provider)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.ProviderUserId)
                .HasMaxLength(128)
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.HasIndex(x => new { x.Provider, x.ProviderUserId })
                .IsUnique()
                .HasDatabaseName("UX_ExternalLogins_Provider");
        }
    }
}
