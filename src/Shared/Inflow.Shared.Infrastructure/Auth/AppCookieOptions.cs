namespace Inflow.Shared.Infrastructure.Auth;

public class AppCookieOptions
{
    public bool HttpOnly { get; set; }
    public bool Secure { get; set; }
    public string SameSite { get; set; }
}