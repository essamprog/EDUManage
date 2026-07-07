using EduManage.Application.Common;
using EduManage.Application.DTOs.Auth;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduManage.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPhotoService _photoService;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IPhotoService photoService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _photoService = photoService;
    }

    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ── GET /Profile ──────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.FindByIdAsync(UserId.ToString());
        if (user is null) return NotFound();

        var dto = new ProfileDto
        {
            FullName  = user.FullName,
            Email     = user.Email ?? string.Empty,
            Bio       = user.Bio,
            Website   = user.Website,
            Linkedin  = user.Linkedin,
            Github    = user.Github,
        };

        ViewData["ProfilePicture"] = user.ProfilePicture;
        ViewData["MemberSince"]    = user.CreatedAt.Year;
        ViewData["Role"]           = (await _userManager.GetRolesAsync(user))
                                        .FirstOrDefault() ?? "User";
        return View(dto);
    }

    // ── POST /Profile/UpdateInfo ──────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInfo(ProfileDto dto)
    {
        if (!ModelState.IsValid)
        {
            var u = await _userManager.FindByIdAsync(UserId.ToString());
            ViewData["ProfilePicture"] = u?.ProfilePicture;
            ViewData["MemberSince"]    = u?.CreatedAt.Year;
            ViewData["Role"]           = (await _userManager.GetRolesAsync(u!))
                                            .FirstOrDefault() ?? "User";
            ViewData["ActiveTab"] = "info";
            return View("Index", dto);
        }

        var user = await _userManager.FindByIdAsync(UserId.ToString());
        if (user is null) return NotFound();

        // Check email uniqueness (skip if same email)
        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing is not null && existing.Id != user.Id)
            {
                TempData["Error"] = "This email address is already in use.";
                return RedirectToAction(nameof(Index));
            }
            user.Email    = dto.Email;
            user.UserName = dto.Email;
        }

        user.FullName  = dto.FullName;
        user.Bio       = dto.Bio;
        user.Website   = dto.Website;
        user.Linkedin  = dto.Linkedin;
        user.Github    = dto.Github;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        // Refresh the auth cookie so the navbar reflects the updated name
        await _signInManager.SignInAsync(user, isPersistent: false);

        TempData["Success"] = "Profile updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    // ── POST /Profile/ChangePassword ──────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"]     = string.Join(" | ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            TempData["ActiveTab"] = "password";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(UserId.ToString());
        if (user is null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(
            user, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            TempData["Error"]     = string.Join(" | ", result.Errors.Select(e => e.Description));
            TempData["ActiveTab"] = "password";
            return RedirectToAction(nameof(Index));
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        TempData["Success"] = "Password changed successfully!";
        return RedirectToAction(nameof(Index));
    }

    // ── POST /Profile/UploadAvatar ────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
    {
        if (avatarFile is null || avatarFile.Length == 0)
        {
            TempData["Error"] = "Please select an image to upload.";
            return RedirectToAction(nameof(Index));
        }

        // Validate file type
        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowed.Contains(avatarFile.ContentType.ToLower()))
        {
            TempData["Error"] = "Only JPEG, PNG, WebP, or GIF images are allowed.";
            return RedirectToAction(nameof(Index));
        }

        // Validate file size (max 5 MB)
        if (avatarFile.Length > 5 * 1024 * 1024)
        {
            TempData["Error"] = "Image size must not exceed 5 MB.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(UserId.ToString());
        if (user is null) return NotFound();

        try
        {
            // Delete old avatar from Cloudinary if exists
            if (!string.IsNullOrEmpty(user.ProfilePictureKey))
                await _photoService.DeletePhotoAsync(user.ProfilePictureKey);

            // Upload new avatar using the standard EduManage storage path
            var (url, publicId) = await _photoService.AddImageAsync(avatarFile, StoragePaths.UserAvatars(UserId));

            user.ProfilePicture    = url;
            user.ProfilePictureKey = publicId;
            user.UpdatedAt         = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Avatar updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Upload failed: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
