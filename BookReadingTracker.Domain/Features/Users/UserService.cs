using BCrypt.Net;
using BookReadingTracker.Database.AppDbContextModels;
using BookReadingTracker.Domain.Features.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookReadingTracker.Domain.Features.Users;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public UserService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var emailExists = await _db.Users
            .AnyAsync(u => u.Email == request.Email && !u.IsDeleted);

        if (emailExists)
            throw new Exception("Email is already registered.");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedDate = DateTime.Now,
            IsDeleted = false
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new RegisterResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role,
            Message = "Registration successful."
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid email or password.");

        var token = GenerateJwtToken(user);

        return new LoginResponse
        {
            Token = token,
            UserId = user.UserId,
            UserName = user.UserName,
            Role = user.Role
        };
    }

    public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request, Guid userId)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

        if (user is null)
            throw new Exception("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            throw new Exception("Old password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ModifiedDate = DateTime.Now;
        user.ModifiedBy = userId;

        await _db.SaveChangesAsync();

        return new ChangePasswordResponse { Message = "Password changed successfully." };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expireDays = int.Parse(_config["Jwt:ExpireDays"] ?? "7");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expireDays),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
