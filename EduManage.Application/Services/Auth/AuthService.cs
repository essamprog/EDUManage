using EduManage.Application.DTOs.Auth;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EduManage.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork uow,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _uow = uow;
        _config = config;
    }

    // ── Register ─────────────────────────────────────────
    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing is not null)
            return new AuthResult { Succeeded = false, Error = "The email address is already in use." };

        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            Status = UserStatus.Active,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return new AuthResult
            {
                Succeeded = false,
                Error = string.Join(", ", result.Errors.Select(e => e.Description))
            };

        await _userManager.AddToRoleAsync(user, "Student");

        return await GenerateTokensAsync(user);
    }

    // ── Login ─────────────────────────────────────────────
    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || user.DeletedAt is not null)
            return new AuthResult { Succeeded = false, Error = "Incorrect data" };

        if (user.Status == UserStatus.Banned)
            return new AuthResult { Succeeded = false, Error = "This account is blocked" };

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return new AuthResult { Succeeded = false, Error = "Incorrect data" };

        return await GenerateTokensAsync(user);
    }

    // ── Refresh Token ─────────────────────────────────────
    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var tokens = await _uow.RefreshTokens
            .FindAsync(rt => rt.Token == refreshToken && rt.ExpiresAt > DateTime.UtcNow);

        var token = tokens.FirstOrDefault();
        if (token is null)
            return new AuthResult { Succeeded = false, Error = "Refresh token is invalid" };

        var user = await _userManager.FindByIdAsync(token.UserId.ToString());
        if (user is null)
            return new AuthResult { Succeeded = false, Error = "User not found" };

        // Rotation — احذف القديم وأنشئ جديد
        _uow.RefreshTokens.Delete(token);
        await _uow.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    // ── Revoke Token ─────────────────────────────────────
    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var tokens = await _uow.RefreshTokens
            .FindAsync(rt => rt.Token == refreshToken);

        var token = tokens.FirstOrDefault();
        if (token is null) return false;

        _uow.RefreshTokens.Delete(token);
        await _uow.SaveChangesAsync();
        return true;
    }

    // ── Forgot Password ───────────────────────────────────
    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return true; // مش بنكشف إن الإيميل مش موجود

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // عضو 3 هيبعت الإيميل هنا عن طريق IEmailService
        return true;
    }

    // ── Reset Password ────────────────────────────────────
    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return false;

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        return result.Succeeded;
    }

    // ── Private Helpers ───────────────────────────────────
    private async Task<AuthResult> GenerateTokensAsync(ApplicationUser user)
    {
        var accessToken = await GenerateJwtAsync(user);
        var refreshToken = GenerateRefreshToken();

        await _uow.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
        });
        await _uow.SaveChangesAsync();

        return new AuthResult
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    private async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email,          user.Email!),
            new(ClaimTypes.Name,           user.FullName),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(int.Parse(_config["JwtSettings:ExpiryDays"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}