using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // CartItemConfiguration.cs
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => new { c.StudentId, c.CourseId }).IsUnique();

            builder.HasOne(c => c.Student)
                   .WithMany(u => u.CartItems)
                   .HasForeignKey(c => c.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Course)
                   .WithMany(co => co.CartItems)
                   .HasForeignKey(c => c.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
