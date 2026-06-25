// ApplicationUserConfiguration.cs
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManage.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FullName).IsRequired().HasMaxLength(120);
        builder.Property(u => u.ProfilePicture).HasMaxLength(255);
        builder.Property(u => u.ProfilePictureKey).HasMaxLength(255);
        builder.Property(u => u.Bio).HasColumnType("text");
        builder.Property(u => u.Website).HasMaxLength(255);
        builder.Property(u => u.Linkedin).HasMaxLength(255);
        builder.Property(u => u.Github).HasMaxLength(255);
        builder.Property(u => u.Status)
               .IsRequired()
               .HasDefaultValue(UserStatus.Active)
               .HasConversion<string>();
        builder.Property(u => u.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        builder.Property(u => u.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        builder.Property(u => u.DeletedAt).IsRequired(false);

        builder.HasIndex(u => u.Status);
        builder.HasIndex(u => u.DeletedAt);
        builder.HasQueryFilter(u => u.DeletedAt == null);
    }
}