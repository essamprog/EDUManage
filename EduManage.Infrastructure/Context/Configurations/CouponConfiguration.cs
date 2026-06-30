using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // CouponConfiguration.cs
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Coupons");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Type).HasConversion<string>().HasDefaultValue(CouponType.Percent);
            builder.Property(c => c.Value).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(c => c.UsedCount).HasDefaultValue(0);
            builder.Property(c => c.IsActive).HasDefaultValue(true);

            builder.HasIndex(c => c.Code).IsUnique();
        }
    }
}
