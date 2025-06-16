using Factory.Models;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Factory.Data;

namespace Factory.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public (bool success, string token) Authenticate(string username, string password)
        {
            Console.WriteLine($"Attempting to authenticate user: {username}");

            var admin = _context.AdminUsers.SingleOrDefault(u => u.Username == username);

            if (admin == null)
            {
                Console.WriteLine("User not found");
                return (false, null);
            }

            Console.WriteLine($"Found user: {admin.Username}");
            Console.WriteLine($"Stored hash: {admin.PasswordHash}");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
            Console.WriteLine($"Password verification result: {isPasswordValid}");

            if (!isPasswordValid)
                return (false, null);

            var token = GenerateJwtToken(admin);
            admin.Token = token;
            admin.TokenExpiry = DateTime.UtcNow.AddDays(1);
            _context.SaveChanges();

            return (true, token);
        }


        public void Logout(string username)
        {
            var admin = _context.AdminUsers.SingleOrDefault(u => u.Username == username);
            if (admin != null)
            {
                admin.Token = null;
                admin.TokenExpiry = null;
                _context.SaveChanges();
            }
        }

        private string GenerateJwtToken(AdminUser admin)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            // Implement password verification (e.g., using BCrypt)
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
