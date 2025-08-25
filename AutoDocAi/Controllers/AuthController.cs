using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoDocAi.Database;
using AutoDocAi.Database.Entities;
using AutoDocAi.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AutoDocAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext appDbContext, IConfiguration config)
        {
            _appDbContext = appDbContext;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.PasswordHash))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var exists = await _appDbContext.Users.AnyAsync(u => u.Username == request.UserName);
            if (exists)
            {
                return Conflict(new { message = "Username already exists." });
            }

            var passwordRecord = HashPassword(request.PasswordHash);

            var user = new User
            {
                Username = request.UserName,
                PasswordHash = passwordRecord,
                Role = "Admin"
            };

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == request.UserName);
            if (user is null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully. Remove the token on the client." });
        }

        private string GenerateJwtToken(User user)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = _config["Jwt:Key"];

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(user.Role) ? "Admin" : user.Role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private static string HashPassword(string password)
        {
            // Salted HMACSHA256 hash stored as: {saltBase64}.{hashBase64}
            var salt = RandomNumberGenerator.GetBytes(16);
            using var hmac = new HMACSHA256(salt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(string password, string passwordRecord)
        {
            var parts = passwordRecord.Split('.', 2);
            if (parts.Length != 2) return false;
            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);
            using var hmac = new HMACSHA256(salt);
            var actualHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
    }
}