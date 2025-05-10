using TodoWebApi.Models;
using TodoWebApi.DTOs;
using TodoWebApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace TodoWebApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User> Register(RegisterUserDto dto)
        {
            using var hmac = new HMACSHA512();

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                passwordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
