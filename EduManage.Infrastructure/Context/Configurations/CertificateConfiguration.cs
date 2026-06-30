using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // CertificateConfiguration.cs
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.ToTable("Certificates");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CertificateUrl).IsRequired().HasMaxLength(255);

            // One-to-One مع Enrollment
            builder.HasIndex(c => c.EnrollmentId).IsUnique();

            builder.HasOne(c => c.Enrollment)
                   .WithOne(e => e.Certificate)
                   .HasForeignKey<Certificate>(c => c.EnrollmentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Student)
                   .WithMany()
                   .HasForeignKey(c => c.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Course)
                   .WithMany(co => co.Certificates)
                   .HasForeignKey(c => c.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
