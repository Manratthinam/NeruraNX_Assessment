using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using neuranx.Application.Interfaces;
using neuranx.Domain;
using neuranx.Domain.Entities;
using neuranx.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace neuranx.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public IdentityService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<object> GetAllUsersAsync()
    {
        var users = await _context.Users.Select(u => new
        {
            u.Id,
            u.UserName,
            u.Email
        }).ToListAsync();

        return users;
    }

    public async Task<AuthResult> LoginUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            Console.WriteLine($"[AuthDebug] User not found for email: '{email}' (Length: {email?.Length})");
            var testUsers = await _context.Users.Where(u => u.Email.Contains("testuser")).Select(u => u.Email).ToListAsync();
            Console.WriteLine($"[AuthDebug] Test Users in DB: {string.Join(", ", testUsers)}");
        }
        else
        {
            var inputHash = HashPassword(password);
            Console.WriteLine($"[AuthDebug] Email: {email}");
            Console.WriteLine($"[AuthDebug] Stored Hash: {user.PasswordHash}");
            Console.WriteLine($"[AuthDebug] Input Hash:  {inputHash}");
            Console.WriteLine($"[AuthDebug] Match: {inputHash == user.PasswordHash}");
        }

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return new AuthResult
            {
                Token = string.Empty,
                RefreshToken = string.Empty,
                Success = false,
                Errors = new List<string> { "Invalid login attempt." },
            };
        }

        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthResult> RefreshTokenAsync(string userId, string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResult { Token = string.Empty, RefreshToken = string.Empty, Success = false, Errors = new List<string> { "Invalid refresh token" } };
        }

        return await GenerateAuthResultAsync(user);
    }

    public async Task<bool> RegisterUserAsync(string userName, string email, string password)
    {
        var existingUser = await _context.Users.AnyAsync(u => u.Email == email);
        if (existingUser)
        {
            return false;
        }

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            PasswordHash = HashPassword(password)
        };

        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<AuthResult> GenerateAuthResultAsync(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        DateTime currentDate = DateTime.UtcNow;
        string refreshToken = user.RefreshToken ?? string.Empty;

        if (user.RefreshTokenExpiryTime > currentDate)
        {
            refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

        }

        return new AuthResult
        {
            Token = jwtToken,
            RefreshToken = refreshToken,
            Success = true,
            ExpiresIn = Convert.ToInt32(jwtSettings["ExpiryMinutes"])
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
