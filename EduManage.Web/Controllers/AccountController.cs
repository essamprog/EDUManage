using EduManage.Application.DTOs.Auth;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduManage.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        IAuthService authService,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // ── Register ─────────────────────────────────────────
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToUserDashboard();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _authService.RegisterAsync(dto);
        if (!result.Succeeded)
        {
            ViewData["Error"] = result.Error;
            return View(dto);
        }

        // تسجيل دخول تلقائي بعد التسجيل
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is not null)
            await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("Index", "Home");
    }

    // ── Login ─────────────────────────────────────────────
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToUserDashboard();

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _authService.LoginAsync(dto);
        if (!result.Succeeded)
        {
            ViewData["Error"] = result.Error;
            ViewData["ReturnUrl"] = returnUrl;
            return View(dto);
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is not null)
            await _signInManager.SignInAsync(user, isPersistent: dto.Remember);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToUserDashboard();
    }

    // ── Logout ────────────────────────────────────────────
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // ── Forgot Password ───────────────────────────────────
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ModelState.AddModelError("", "Email address required");
            return View();
        }

        await _authService.ForgotPasswordAsync(email);
        ViewData["Success"] = true;
        return View();
    }

    // ── Reset Password ────────────────────────────────────
    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            return RedirectToAction("Login");

        var dto = new ResetPasswordDto { Email = email, Token = token };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _authService.ResetPasswordAsync(dto);
        if (!result)
        {
            ViewData["Error"] = "Password reset failed, please check the link.";
            return View(dto);
        }

        TempData["Success"] = "Password changed successfully";
        return RedirectToAction("Login");
    }

    // ── Helper ────────────────────────────────────────────
    private IActionResult RedirectToUserDashboard()
    {
        if (User.IsInRole("Admin"))
            return RedirectToAction("Index", "Admin");

        if (User.IsInRole("Instructor"))
            return RedirectToAction("Index", "Instructor");

        return RedirectToAction("Index", "Dashboard");
    }
}