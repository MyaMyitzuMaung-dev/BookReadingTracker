namespace BookReadingTracker.MVC.Models.Auth;

public class LoginViewModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterViewModel
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "User";
}

public class ChangePasswordViewModel
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}
