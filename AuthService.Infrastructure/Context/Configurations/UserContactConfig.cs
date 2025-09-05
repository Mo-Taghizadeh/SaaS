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
    public class UserContactConfig : IEntityTypeConfiguration<UserContact>
    {
        public void Configure(EntityTypeBuilder<UserContact> b)
        {
            b.ToTable("UserContacts", "auth");
            b.HasKey(x => x.Id);

            b.Property(x => x.Type)
                .HasConversion<byte>()
                .IsRequired();

            b.Property(x => x.Value)
                .HasMaxLength(320)    // ایمیل تا 320، موبایل E.164 تا ~ 20
                .IsRequired();

            // EmailLower فقط برای Email کاربرد دارد
            b.Property(x => x.EmailLower)
                .HasMaxLength(320)
                .HasComputedColumnSql("LOWER([Value])", stored: true);

            b.Property(x => x.IsPrimary).HasDefaultValue(false);
            b.Property(x => x.IsVerified).HasDefaultValue(false);

            b.Property(x => x.VerifiedAt)
                .HasColumnType("datetime2");

            b.HasIndex(x => new { x.UserId, x.Type, x.IsPrimary })
                .HasDatabaseName("IX_UserContacts_User_Primary");

            // یونیک ایمیل: فقط برای Email (Type=1) روی EmailLower
            b.HasIndex(x => x.EmailLower)
                .IsUnique()
                .HasFilter("[Type] = 1 AND [EmailLower] IS NOT NULL")
                .HasDatabaseName("UX_UserContacts_Email");

            // یونیک موبایل: فقط برای Mobile (Type=2) روی Value (E.164)
            b.HasIndex(x => x.Value)
                .IsUnique()
                .HasFilter("[Type] = 2")
                .HasDatabaseName("UX_UserContacts_Mobile");
        }
    }
}
