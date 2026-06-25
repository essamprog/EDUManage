using EduManage.Core.Entities;
using EduManage.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManage.Infrastructure.Context.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // ── Table ────────────────────────────────────────────
        builder.ToTable("Users");

        // ── Properties ───────────────────────────────────────
        builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.ProfilePicture)
               .HasMaxLength(500);

        builder.Property(u => u.ProfilePictureKey)
               .HasMaxLength(200);

        builder.Property(u => u.Bio)
               .HasMaxLength(1000);

        builder.Property(u => u.Website)
               .HasMaxLength(200);

        builder.Property(u => u.Linkedin)
               .HasMaxLength(200);

        builder.Property(u => u.Github)
               .HasMaxLength(200);

        builder.Property(u => u.Status)
               .IsRequired()
               .HasDefaultValue(UserStatus.Active)
               .HasConversion<string>();   // يخزن "Active" مش 1

        builder.Property(u => u.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.UpdatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.DeletedAt)
               .IsRequired(false);

        // ── Indexes ──────────────────────────────────────────
        builder.HasIndex(u => u.Email)
               .IsUnique();

        builder.HasIndex(u => u.Status);

        builder.HasIndex(u => u.DeletedAt);   // لتسريع الـ Soft Delete Filter

        // ── Relationships ────────────────────────────────────

        // User → InstructorProfile (One-to-One)
        builder.HasOne(u => u.InstructorProfile)
               .WithOne(ip => ip.User)
               .HasForeignKey<InstructorProfile>(ip => ip.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // User → RefreshTokens (One-to-Many)
        builder.HasMany(u => u.RefreshTokens)
               .WithOne(rt => rt.User)
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        /*
        // User → Orders (One-to-Many)
        builder.HasMany(u => u.Orders)
               .WithOne(o => o.Student)
               .HasForeignKey(o => o.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        // User → Enrollments (One-to-Many)
        builder.HasMany(u => u.Enrollments)
               .WithOne(e => e.Student)
               .HasForeignKey(e => e.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        // User → Reviews (One-to-Many)
        builder.HasMany(u => u.Reviews)
               .WithOne(r => r.Student)
               .HasForeignKey(r => r.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        // User → Notifications (One-to-Many)
        builder.HasMany(u => u.Notifications)
               .WithOne(n => n.User)
               .HasForeignKey(n => n.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // User → InstructorApplications (One-to-Many)
        builder.HasMany(u => u.Applications)
               .WithOne(a => a.User)
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        */
    }
}