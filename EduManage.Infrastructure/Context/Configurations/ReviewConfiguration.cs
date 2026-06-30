using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // ReviewConfiguration.cs
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasColumnType("text");

            // Unique: طالب + كورس — تقييم واحد بس
            builder.HasIndex(r => new { r.StudentId, r.CourseId }).IsUnique();

            builder.HasOne(r => r.Student)
                   .WithMany(u => u.Reviews)
                   .HasForeignKey(r => r.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Course)
                   .WithMany(c => c.Reviews)
                   .HasForeignKey(r => r.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
