using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // LessonResourceConfiguration.cs
    public class LessonResourceConfiguration : IEntityTypeConfiguration<LessonResource>
    {
        public void Configure(EntityTypeBuilder<LessonResource> builder)
        {
            builder.ToTable("LessonResources");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.FileName).IsRequired().HasMaxLength(255);
            builder.Property(r => r.FileUrl).IsRequired().HasMaxLength(500);
            builder.Property(r => r.FileKey).HasMaxLength(500);
            builder.Property(r => r.FileType).IsRequired().HasMaxLength(50);

            builder.HasOne(r => r.Lesson)
                   .WithMany(l => l.Resources)
                   .HasForeignKey(r => r.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
