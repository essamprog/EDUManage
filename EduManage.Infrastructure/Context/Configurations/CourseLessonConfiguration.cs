using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // CourseLessonConfiguration.cs
    public class CourseLessonConfiguration : IEntityTypeConfiguration<CourseLesson>
    {
        public void Configure(EntityTypeBuilder<CourseLesson> builder)
        {
            builder.ToTable("CourseLessons");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Title).IsRequired().HasMaxLength(255);
            builder.Property(l => l.VideoPath).HasMaxLength(500);
            builder.Property(l => l.VideoKey).HasMaxLength(500);
            builder.Property(l => l.VideoStatus).HasConversion<string>().HasDefaultValue(VideoStatus.Ready);
            builder.Property(l => l.Status).HasConversion<string>().HasDefaultValue(LessonStatus.Draft);

            builder.HasIndex(l => new { l.SectionId, l.OrderIndex });
            builder.HasIndex(l => new { l.DeletedAt, l.Status });
            builder.HasQueryFilter(l => l.DeletedAt == null);

            builder.HasOne(l => l.Section)
                   .WithMany(s => s.Lessons)
                   .HasForeignKey(l => l.SectionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
