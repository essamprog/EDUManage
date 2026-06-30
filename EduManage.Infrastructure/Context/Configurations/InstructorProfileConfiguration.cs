using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // InstructorProfileConfiguration.cs
    public class InstructorProfileConfiguration : IEntityTypeConfiguration<InstructorProfile>
    {
        public void Configure(EntityTypeBuilder<InstructorProfile> builder)
        {
            builder.ToTable("InstructorProfiles");
            builder.HasKey(ip => ip.UserId);  // UserId هو الـ PK مباشرة
            builder.Property(ip => ip.Bio).HasColumnType("text");
            builder.Property(ip => ip.Expertise).HasMaxLength(150);
            builder.Property(ip => ip.Experience).HasMaxLength(100);
            builder.Property(ip => ip.AverageRating).HasColumnType("decimal(3,2)").HasDefaultValue(0);
            builder.Property(ip => ip.TotalStudents).HasDefaultValue(0);
        }
    }
}
