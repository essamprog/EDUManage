using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // LessonProgressConfiguration.cs
    public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            builder.ToTable("LessonProgress");
            builder.HasKey(lp => lp.Id);
            builder.Property(lp => lp.WatchTime).HasDefaultValue(0);

            // Unique: طالب + درس
            builder.HasIndex(lp => new { lp.StudentId, lp.LessonId }).IsUnique();

            builder.HasOne(lp => lp.Student)
                   .WithMany()
                   .HasForeignKey(lp => lp.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lp => lp.Lesson)
                   .WithMany(l => l.Progresses)
                   .HasForeignKey(lp => lp.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lp => lp.Course)
                   .WithMany()
                   .HasForeignKey(lp => lp.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
