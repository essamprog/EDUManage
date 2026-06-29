using System;
using System.Collections.Generic;
using System.Text;

// Auth/AuthResult.cs
namespace EduManage.Application.DTOs.Auth;

public class AuthResult
{
    public bool Succeeded { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? Error { get; set; }
}
