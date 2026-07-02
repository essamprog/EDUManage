// Application/Services/System/EmailService.cs
using EduManage.Application.DTOs.Financial;
using EduManage.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EduManage.Application.Services.System;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public async Task SendWelcomeAsync(string email, string fullName)
        => await SendAsync(
            to: email,
            subject: $"Welcome to EduManage, {fullName}",
            body: $"""
                      <div style="font-family:Arial;padding:20px">
                        <h2>Welcome, {fullName} 🎉</h2>
                        <p>We are thrilled to have you join the EduManage platform.</p>
                        <p>Start your learning journey now and browse hundreds of courses.</p>
                      </div>
                      """);

    public async Task SendPasswordResetAsync(string email, string resetLink)
        => await SendAsync(
            to: email,
            subject: "Password Reset",
            body: $"""
                      <div style="font-family:Arial;padding:20px">
                        <h2>Password Reset</h2>
                        <p>Click the link below to reset your password:</p>
                        <a href="{resetLink}" style="background:#0d6efd;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none">
                          Reset Password
                        </a>
                        <p style="color:#888;font-size:12px;margin-top:16px">
                          This link is valid for 24 hours only.
                        </p>
                      </div>
                      """);

    public async Task SendOrderConfirmationAsync(string email, OrderDto order)
        => await SendAsync(
            to: email,
            subject: $"Order Confirmation #{order.Id}",
            body: $"""
                      <div style="font-family:Arial;padding:20px">
                        <h2>Your order has been successfully confirmed ✅</h2>
                        <p>Order Number: <strong>#{order.Id}</strong></p>
                        <p>Total: <strong>{order.TotalAmount} EGP</strong></p>
                        <p>You can now access your courses from your account.</p>
                      </div>
                      """);

    public async Task SendCertificateAsync(string email, string certificateUrl)
        => await SendAsync(
            to: email,
            subject: "Congratulations! Your certificate is ready 🎓",
            body: $"""
                      <div style="font-family:Arial;padding:20px">
                        <h2>Congratulations on completing the course! 🎓</h2>
                        <p>Your certificate is ready to download:</p>
                        <a href="{certificateUrl}" style="background:#198754;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none">
                          Download Certificate
                        </a>
                      </div>
                      """);

    // ── Private Helper ────────────────────────────────────
    private async Task SendAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();

        // حطينا ! هنا
        message.From.Add(new MailboxAddress(
            _config["EmailSettings:FromName"]!,
            _config["EmailSettings:FromEmail"]!));

        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        // حطينا ! عند الـ Host
        await smtp.ConnectAsync(
            _config["EmailSettings:Host"]!,
            int.Parse(_config["EmailSettings:Port"]!),
            SecureSocketOptions.StartTls);

        // حطينا ! عند اليوزر والباسورد
        await smtp.AuthenticateAsync(
            _config["EmailSettings:Username"]!,
            _config["EmailSettings:Password"]!);

        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}