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
    public class VerificationTokenConfig : IEntityTypeConfiguration<VerificationToken>
    {
        public void Configure(EntityTypeBuilder<VerificationToken> b)
        {
            b.ToTable("VerificationTokens", "auth");
            b.HasKey(x => x.Id);

            b.Property(x => x.Purpose)
                .HasConversion<byte>()
                .IsRequired();

            b.Property(x => x.ContactType)
                .HasConversion<byte>()
                .IsRequired();

            b.Property(x => x.ContactValue)
                .HasMaxLength(320)
                .IsRequired();

            b.Property(x => x.TokenHash)
                .HasMaxLength(256)
                .IsRequired();

            b.Property(x => x.ExpiresAt)
                .HasColumnType("datetime2");

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UsedAt)
                .HasColumnType("datetime2");

            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // برای جست‌وجو/پاکسازی‌های زمان‌دار
            b.HasIndex(x => new { x.Purpose, x.ContactType, x.ContactValue })
                .HasDatabaseName("IX_Verify_PurposeContact");

            // توکن‌های معتبرِ منقضی‌نشده
            b.HasIndex(x => new { x.ContactType, x.ContactValue, x.ExpiresAt })
                .HasFilter("[UsedAt] IS NULL")
                .HasDatabaseName("IX_Verify_Active");
        }
    }
}
