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
            // HMACSHA512 generates a secure random key(salt) 
            using var hmac = new HMACSHA512();

            // The plaintext password is hashed used the HMACSHA512 algorithm and converts the pw to a byte array
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                passwordSalt = hmac.Key // Store the salt it can be reused for verification during login
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            // hash the plaintext pw based on the salt
            using var hmac = new HMACSHA512(user.passwordSalt);
            // convert to byte array
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            // verify ecah bype of computed hash with stored hash
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return null;
            }

            return user;
        }
    }
}
