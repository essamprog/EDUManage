using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // InstructorApplicationConfiguration.cs
    public class InstructorApplicationConfiguration : IEntityTypeConfiguration<InstructorApplication>
    {
        public void Configure(EntityTypeBuilder<InstructorApplication> builder)
        {
            builder.ToTable("InstructorApplications");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Expertise).IsRequired().HasMaxLength(150);
            builder.Property(a => a.Experience).IsRequired().HasMaxLength(100);
            builder.Property(a => a.ResumeUrl).HasMaxLength(255);
            builder.Property(a => a.Status).HasConversion<string>().HasDefaultValue(ApplicationStatus.Pending);
        }
    }
}
