using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // CourseConfiguration.cs
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title).IsRequired().HasMaxLength(255);
            builder.Property(c => c.Subtitle).HasMaxLength(500);
            builder.Property(c => c.Description).HasColumnType("text");
            builder.Property(c => c.Level).IsRequired().HasConversion<string>().HasDefaultValue(CourseLevel.Beginner);
            builder.Property(c => c.Status).IsRequired().HasConversion<string>().HasDefaultValue(CourseStatus.Draft);
            builder.Property(c => c.Price).IsRequired().HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(c => c.OriginalPrice).HasColumnType("decimal(10,2)");
            builder.Property(c => c.ThumbnailUrl).HasMaxLength(500);
            builder.Property(c => c.ThumbnailKey).HasMaxLength(500);
            builder.Property(c => c.PromoVideoUrl).HasMaxLength(500);
            builder.Property(c => c.PromoVideoKey).HasMaxLength(500);
            builder.Property(c => c.AverageRating).HasColumnType("decimal(3,2)").HasDefaultValue(0);
            builder.Property(c => c.Language).IsRequired().HasMaxLength(50).HasDefaultValue("Arabic");

            // Indexes
            builder.HasIndex(c => new { c.Status, c.CategoryId, c.IsBestseller });
            builder.HasIndex(c => new { c.DeletedAt, c.Status });
            builder.HasQueryFilter(c => c.DeletedAt == null);

            // Relationships
            builder.HasOne(c => c.Instructor)
                   .WithMany(ip => ip.Courses)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Category)
                   .WithMany(cat => cat.Courses)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
