using TodoWebApi.Models;
using TodoWebApi.DTOs;
using TodoWebApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace TodoWebApi.Services
{
    public class AuthService
    {
        // Todo - Gradually move from existing implementation to UserManager
        // A list of possible changes would be
        /// <summary>
        /// Combine ApplicationUser and Identity 
        /// JWT auth logic - As long as we're only issuing tokens based on our rules and data there is no need for change
        /// Controllers & Services - Could move away from old User class to consistent use of ApplicationUser
        /// Token validation pipeline - Our middleware just validates tokens based on claims, not Identity, so no need for change
        /// In summary - We have the implementation ready to go when we want to refactor/scale with complexity, features and protection.
        /// But for now its OK.
        /// </summary>

        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AuthService(
            //UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            //_userManager = userManager;
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
