using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // AuditLogConfiguration.cs
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(255);
            builder.Property(a => a.EntityType).HasMaxLength(100);
            builder.Property(a => a.Details).HasColumnType("text");

            builder.HasOne(a => a.Admin)
                   .WithMany()
                   .HasForeignKey(a => a.AdminId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
