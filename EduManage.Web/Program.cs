using EduManage.Application.Interfaces;
using EduManage.Application.Services.Auth;
using EduManage.Application.Services.Courses;
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
            // Program.cs
            var builder = WebApplication.CreateBuilder(args);

            // ── Database ─────────────────────────────────────────────
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ── Identity ─────────────────────────────────────────────
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

            // ── Services ─────────────────────────────────────────────
            // عضو 2 هيضيف هنا:
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICourseService, CourseService>();

            // عضو 3 هيضيف هنا:
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IWalletService, WalletService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<SearchService, SearchService>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // ── Seed ─────────────────────────────────────────────────
            await DbInitializer.SeedAsync(app.Services);

            // ── Middleware ───────────────────────────────────────────
            if (!app.Environment.IsDevelopment())
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // ── Areas ────────────────────────────────────────────────
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
