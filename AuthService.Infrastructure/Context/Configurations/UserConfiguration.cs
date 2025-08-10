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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            // جدول و اسکیمـا
            b.ToTable("Users", schema: "auth");

            // کلید اصلی + تولید مقدار سمت SQL Server
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
             .HasColumnType("uniqueidentifier")
             .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Username
            b.Property(u => u.Username)
             .IsRequired()
             .HasMaxLength(50);

            // NormalizedUsername (برای یکتا)
            b.Property(u => u.NormalizedUsername)
             .IsRequired()
             .HasMaxLength(50);

            // Email
            b.Property(u => u.Email)
             .IsRequired()
             .HasMaxLength(320); // مطابق RFC (local-part تا 64 + domain تا 255)

            // NormalizedEmail (برای یکتا)
            b.Property(u => u.NormalizedEmail)
             .IsRequired()
             .HasMaxLength(320);

            // PasswordHash
            b.Property(u => u.PasswordHash)
             .IsRequired()
             .HasMaxLength(256); // بستگی به الگوریتمت؛ 256 برای Argon2/Bcrypt Base64 معمولاً کافیه

            // IsActive
            b.Property(u => u.IsActive)
             .HasDefaultValue(true)
             .IsRequired();

            // CreatedAt: مقدار پیش‌فرض UTC از SQL
            b.Property(u => u.CreatedAt)
             .HasColumnType("datetime2")
             .HasDefaultValueSql("SYSUTCDATETIME()")
             .IsRequired();

            // ModifiedAt 
            b.Property(u => u.ModifiedAt)
             .HasColumnType("datetime2")
             .HasDefaultValue(null);

            // RowVersion (کنترل همزمانی)
            b.Property(u => u.RowVersion)
             .IsRowVersion()
             .IsConcurrencyToken();

            // ایندکس‌های منحصربه‌فرد (Case-insensitive با Normalized*)
            b.HasIndex(u => u.NormalizedUsername)
             .HasDatabaseName("UX_Users_NormalizedUsername")
             .IsUnique();

            b.HasIndex(u => u.NormalizedEmail)
             .HasDatabaseName("UX_Users_NormalizedEmail")
             .IsUnique();

            // ایندکس کمکی برای گزارش‌گیری/مرتب‌سازی بر اساس تاریخ
            b.HasIndex(u => u.CreatedAt)
             .HasDatabaseName("IX_Users_CreatedAt");
        }
    }
}
