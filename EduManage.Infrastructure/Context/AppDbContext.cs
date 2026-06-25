// ApplicationUser.cs
using CloudinaryDotNet;
using EduManage.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EduManage.Infrastructure.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── DbSets ────────────────────────────────────────────
    public DbSet<InstructorProfile> InstructorProfiles { get; set; }
    public DbSet<InstructorApplication> InstructorApplications { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseSection> CourseSections { get; set; }
    public DbSet<CourseLesson> CourseLessons { get; set; }
    public DbSet<LessonResource> LessonResources { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<QaQuestion> QaQuestions { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);  // مهم جداً — Identity بيحتاجه

        // تطبيق كل الـ Configurations من الـ Assembly دفعة واحدة
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Soft Delete Global Filter
        builder.Entity<ApplicationUser>()
               .HasQueryFilter(u => u.DeletedAt == null);
    }
}