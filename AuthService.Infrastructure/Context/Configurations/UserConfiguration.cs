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
            b.ToTable("Users", "auth");
            b.HasKey(x => x.Id);

            b.Property(x => x.Username)
                .HasMaxLength(50);

            b.Property(x => x.Status)
                .HasConversion<byte>()
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.ModifiedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            // ایندکس اختیاری روی Username (غیر یونیک چون ممکنه null)
            b.HasIndex(x => x.Username)
             .HasDatabaseName("IX_Users_Username");

            // روابط
            b.HasMany(x => x.Contacts)
             .WithOne(x => x.User)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Credential)
             .WithOne(x => x.User)
             .HasForeignKey<UserCredential>(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.ExternalLogins)
             .WithOne(x => x.User)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
