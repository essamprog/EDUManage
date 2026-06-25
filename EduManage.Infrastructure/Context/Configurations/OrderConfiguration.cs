using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // OrderConfiguration.cs
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(o => o.Discount).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(o => o.CouponCode).HasMaxLength(50);
            builder.Property(o => o.PaymentMethod).HasMaxLength(50).HasDefaultValue("paymob");
            builder.Property(o => o.Status).HasConversion<string>().HasDefaultValue(OrderStatus.Pending);
            builder.Property(o => o.TransactionId).HasMaxLength(255);

            builder.HasOne(o => o.Student)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
