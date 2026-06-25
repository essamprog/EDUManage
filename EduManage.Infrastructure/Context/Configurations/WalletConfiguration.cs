using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // WalletConfiguration.cs
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.AvailableBalance).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(w => w.PendingBalance).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(w => w.TotalWithdrawn).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(w => w.LifetimeEarnings).HasColumnType("decimal(10,2)").HasDefaultValue(0);

            // Unique: كل Instructor عنده Wallet واحدة بس
            builder.HasIndex(w => w.InstructorId).IsUnique();

            builder.HasOne(w => w.Instructor)
                   .WithOne(ip => ip.Wallet)
                   .HasForeignKey<Wallet>(w => w.InstructorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
