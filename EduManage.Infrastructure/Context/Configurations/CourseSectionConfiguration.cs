using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // CourseSectionConfiguration.cs
    public class CourseSectionConfiguration : IEntityTypeConfiguration<CourseSection>
    {
        public void Configure(EntityTypeBuilder<CourseSection> builder)
        {
            builder.ToTable("CourseSections");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Title).IsRequired().HasMaxLength(255);
            builder.Property(s => s.OrderIndex).IsRequired();

            builder.HasIndex(s => new { s.CourseId, s.OrderIndex });
            builder.HasIndex(s => s.DeletedAt);
            builder.HasQueryFilter(s => s.DeletedAt == null);

            builder.HasOne(s => s.Course)
                   .WithMany(c => c.Sections)
                   .HasForeignKey(s => s.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
