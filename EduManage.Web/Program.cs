using AutoMapper;
using EduManage.Application.DTOs.System;
using EduManage.Application.Interfaces;
using EduManage.Application.Mappings;
using EduManage.Application.Services.Auth;
using EduManage.Application.Services.Courses;
using EduManage.Application.Services.Enrollment;
using EduManage.Application.Services.Financial;
using EduManage.Application.Services.System;
using EduManage.Core.Entities;
using EduManage.Core.Interfaces;
using EduManage.Infrastructure;
using EduManage.Infrastructure.Context;
using EduManage.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduManage.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── 1. Database ─────────────────────────────────────────────
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ── 2. Identity ─────────────────────────────────────────────
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Auth & Courses
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

            // Financial & Orders
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IWalletService, WalletService>();

            // System & Utilities
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<SearchService, SearchService>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<CourseMappingProfile>();
            });

            builder.Services.AddControllersWithViews();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            var app = builder.Build();

            // ── 8. Data Seeding ─────────────────────────────────────────
            await DbInitializer.SeedAsync(app.Services);

            // ── 9. Middleware Pipeline ──────────────────────────────────
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // ── 10. Routing Configuration

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}