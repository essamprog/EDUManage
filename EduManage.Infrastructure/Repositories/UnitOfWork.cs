using EduManage.Core.Entities;
using EduManage.Core.Interfaces;
using EduManage.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

// Repositories/UnitOfWork.cs
namespace EduManage.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context) => _context = context;

    // ── Lazy initialization لكل Repository ──────────────────
    private IGenericRepository<ApplicationUser>? _users;
    private IGenericRepository<Course>? _courses;
    private IGenericRepository<CourseSection>? _courseSections;
    private IGenericRepository<CourseLesson>? _courseLessons;
    private IGenericRepository<LessonResource>? _lessonResources;
    private IGenericRepository<Category>? _categories;
    private IGenericRepository<Enrollment>? _enrollments;
    private IGenericRepository<LessonProgress>? _lessonProgresses;
    private IGenericRepository<Certificate>? _certificates;
    private IGenericRepository<Review>? _reviews;
    private IGenericRepository<QaQuestion>? _qaQuestions;
    private IGenericRepository<CartItem>? _cartItems;
    private IGenericRepository<Wishlist>? _wishlists;
    private IGenericRepository<Order>? _orders;
    private IGenericRepository<OrderItem>? _orderItems;
    private IGenericRepository<Transaction>? _transactions;
    private IGenericRepository<Wallet>? _wallets;
    private IGenericRepository<Withdrawal>? _withdrawals;
    private IGenericRepository<Coupon>? _coupons;
    private IGenericRepository<Notification>? _notifications;
    private IGenericRepository<AuditLog>? _auditLogs;
    private IGenericRepository<Setting>? _settings;
    private IGenericRepository<InstructorProfile>? _instructorProfiles;
    private IGenericRepository<InstructorApplication>? _instructorApplications;
    private IGenericRepository<RefreshToken>? _refreshTokens;

    public IGenericRepository<ApplicationUser> Users => _users ??= new GenericRepository<ApplicationUser>(_context);
    public IGenericRepository<Course> Courses => _courses ??= new GenericRepository<Course>(_context);
    public IGenericRepository<CourseSection> CourseSections => _courseSections ??= new GenericRepository<CourseSection>(_context);
    public IGenericRepository<CourseLesson> CourseLessons => _courseLessons ??= new GenericRepository<CourseLesson>(_context);
    public IGenericRepository<LessonResource> LessonResources => _lessonResources ??= new GenericRepository<LessonResource>(_context);
    public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
    public IGenericRepository<Enrollment> Enrollments => _enrollments ??= new GenericRepository<Enrollment>(_context);
    public IGenericRepository<LessonProgress> LessonProgresses => _lessonProgresses ??= new GenericRepository<LessonProgress>(_context);
    public IGenericRepository<Certificate> Certificates => _certificates ??= new GenericRepository<Certificate>(_context);
    public IGenericRepository<Review> Reviews => _reviews ??= new GenericRepository<Review>(_context);
    public IGenericRepository<QaQuestion> QaQuestions => _qaQuestions ??= new GenericRepository<QaQuestion>(_context);
    public IGenericRepository<CartItem> CartItems => _cartItems ??= new GenericRepository<CartItem>(_context);
    public IGenericRepository<Wishlist> Wishlists => _wishlists ??= new GenericRepository<Wishlist>(_context);
    public IGenericRepository<Order> Orders => _orders ??= new GenericRepository<Order>(_context);
    public IGenericRepository<OrderItem> OrderItems => _orderItems ??= new GenericRepository<OrderItem>(_context);
    public IGenericRepository<Transaction> Transactions => _transactions ??= new GenericRepository<Transaction>(_context);
    public IGenericRepository<Wallet> Wallets => _wallets ??= new GenericRepository<Wallet>(_context);
    public IGenericRepository<Withdrawal> Withdrawals => _withdrawals ??= new GenericRepository<Withdrawal>(_context);
    public IGenericRepository<Coupon> Coupons => _coupons ??= new GenericRepository<Coupon>(_context);
    public IGenericRepository<Notification> Notifications => _notifications ??= new GenericRepository<Notification>(_context);
    public IGenericRepository<AuditLog> AuditLogs => _auditLogs ??= new GenericRepository<AuditLog>(_context);
    public IGenericRepository<Setting> Settings => _settings ??= new GenericRepository<Setting>(_context);
    public IGenericRepository<InstructorProfile> InstructorProfiles => _instructorProfiles ??= new GenericRepository<InstructorProfile>(_context);
    public IGenericRepository<InstructorApplication> InstructorApplications => _instructorApplications ??= new GenericRepository<InstructorApplication>(_context);
    public IGenericRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new GenericRepository<RefreshToken>(_context);

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}