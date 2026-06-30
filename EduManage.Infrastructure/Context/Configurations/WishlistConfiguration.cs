using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // WishlistConfiguration.cs
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("Wishlists");
            builder.HasKey(w => w.Id);

            builder.HasOne(w => w.Student)
                   .WithMany(u => u.Wishlists)
                   .HasForeignKey(w => w.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.Course)
                   .WithMany(c => c.Wishlists)
                   .HasForeignKey(w => w.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
