using EduManage.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

// IUnitOfWork.cs
namespace EduManage.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<ApplicationUser> Users { get; }
    IGenericRepository<Course> Courses { get; }
    IGenericRepository<CourseSection> CourseSections { get; }
    IGenericRepository<CourseLesson> CourseLessons { get; }
    IGenericRepository<LessonResource> LessonResources { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<Enrollment> Enrollments { get; }
    IGenericRepository<LessonProgress> LessonProgresses { get; }
    IGenericRepository<Certificate> Certificates { get; }
    IGenericRepository<Review> Reviews { get; }
    IGenericRepository<QaQuestion> QaQuestions { get; }
    IGenericRepository<CartItem> CartItems { get; }
    IGenericRepository<Wishlist> Wishlists { get; }
    IGenericRepository<Order> Orders { get; }
    IGenericRepository<OrderItem> OrderItems { get; }
    IGenericRepository<Transaction> Transactions { get; }
    IGenericRepository<Wallet> Wallets { get; }
    IGenericRepository<Withdrawal> Withdrawals { get; }
    IGenericRepository<Coupon> Coupons { get; }
    IGenericRepository<Notification> Notifications { get; }
    IGenericRepository<AuditLog> AuditLogs { get; }
    IGenericRepository<Setting> Settings { get; }
    IGenericRepository<InstructorProfile> InstructorProfiles { get; }
    IGenericRepository<InstructorApplication> InstructorApplications { get; }
    IGenericRepository<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync();
}
