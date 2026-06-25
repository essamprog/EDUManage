using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // EnrollmentConfiguration.cs
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Progress).IsRequired().HasDefaultValue((byte)0);
            builder.Property(e => e.Status).HasConversion<string>().HasDefaultValue(EnrollmentStatus.InProgress);

            // Unique: طالب مينفعش يتسجل في نفس الكورس مرتين
            builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            builder.HasOne(e => e.User)
                   .WithMany(u => u.Enrollments)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
