using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // SettingConfiguration.cs
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Settings");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.SettingKey).IsRequired().HasMaxLength(100);
            builder.Property(s => s.SettingValue).HasColumnType("text");
            builder.HasIndex(s => s.SettingKey).IsUnique();
        }
    }
}
