using EduManage.Application.DTOs.Financial;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Application.Interfaces;

public interface IEmailService
{
    Task SendWelcomeAsync(string email, string fullName);
    Task SendPasswordResetAsync(string email, string resetLink);
    Task SendOrderConfirmationAsync(string email, OrderDto order);
    Task SendCertificateAsync(string email, string certificateUrl);
}