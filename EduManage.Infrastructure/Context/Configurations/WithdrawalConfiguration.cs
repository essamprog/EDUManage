using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // WithdrawalConfiguration.cs
    public class WithdrawalConfiguration : IEntityTypeConfiguration<Withdrawal>
    {
        public void Configure(EntityTypeBuilder<Withdrawal> builder)
        {
            builder.ToTable("Withdrawals");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Amount).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(w => w.Currency).HasMaxLength(10).HasDefaultValue("EGP");
            builder.Property(w => w.Method).IsRequired().HasMaxLength(50);
            builder.Property(w => w.AccountDetail).IsRequired().HasMaxLength(255);
            builder.Property(w => w.Status).HasConversion<string>().HasDefaultValue(WithdrawalStatus.Pending);

            builder.HasOne(w => w.Instructor)
                   .WithMany(ip => ip.Withdrawals)
                   .HasForeignKey(w => w.InstructorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
