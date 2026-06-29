using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

// DbInitializer.cs
namespace EduManage.Infrastructure.Context;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await context.Database.MigrateAsync();

        // ── Roles ────────────────────────────────────────────
        string[] roles = ["Admin", "Instructor", "Student"];
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));

        // ── Admin User ───────────────────────────────────────
        const string adminEmail = "admin@edumanage.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                FullName = "System Admin",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Status = UserStatus.Active,
            };
            var result = await userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── Categories ───────────────────────────────────────
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Programming and Development", Slug = "programming" },
                new Category { Name = "Graphic Design", Slug = "design" },
                new Category { Name = "Business and Entrepreneurship", Slug = "business" }
            );
            await context.SaveChangesAsync();
        }

        // ── Settings ─────────────────────────────────────────
        if (!context.Settings.Any())
        {
            context.Settings.AddRange(
                new Setting { SettingKey = "platform_fee_percent", SettingValue = "20" },
                new Setting { SettingKey = "payout_hold_days", SettingValue = "14" },
                new Setting { SettingKey = "site_name", SettingValue = "EduManage" },
                new Setting { SettingKey = "currency", SettingValue = "EGP" }
            );
            await context.SaveChangesAsync();
        }
    }
}
