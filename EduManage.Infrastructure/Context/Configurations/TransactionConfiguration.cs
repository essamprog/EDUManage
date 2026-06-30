using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduManage.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // TransactionConfiguration.cs
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.GrossAmount).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(t => t.NetAmount).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(t => t.Currency).HasMaxLength(10).HasDefaultValue("EGP");
            builder.Property(t => t.Description).HasMaxLength(255);
            builder.Property(t => t.Type).HasConversion<string>().HasDefaultValue(TransactionType.Sale);
            builder.Property(t => t.Status).HasConversion<string>().HasDefaultValue(TransactionStatus.Pending);

            builder.HasIndex(t => new { t.Status, t.Type, t.CreatedAt });

            builder.HasOne(t => t.Instructor)
                   .WithMany()
                   .HasForeignKey(t => t.InstructorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.OrderItem)
                   .WithMany(oi => oi.Transactions)
                   .HasForeignKey(t => t.OrderItemId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
