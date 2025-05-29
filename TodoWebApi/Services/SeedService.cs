using Bogus;
using TodoWebApi.Models;
using TodoWebApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace TodoWebApi.Services
{
    public static class SeedService
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // If there already is existing data, do nothing.
            // Else, generate users, categories and todos.
            if (await context.Users.AnyAsync()) return;
            

            // Generate users
            var users = new List<User>();
            var password = "Password123!";
            foreach (var username in new[] { "davidsmith", "johndoe", "lukeskywalker" })
            {
                CreatePasswordHash(password, out byte[] hash, out byte[] salt);
                users.Add(new User
                {
                    Username = username,
                    PasswordHash = hash,
                    passwordSalt = salt
                });
            }
            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Generate categories
            var categories = new List<Category>
            {
                new() { Name = "Social", UserId = users[0].Id },
                new() { Name = "Furniture", UserId = users[0].Id },
                new() { Name = "Coding", UserId = users[0].Id },
                new() { Name = "Analyzing", UserId = users[0].Id },
                new() { Name = "Sales", UserId = users[0].Id },
                new() { Name = "Celebration", UserId = users[0].Id },
                new() { Name = "Coding", UserId = users[1].Id },
                new() { Name = "Food", UserId = users[1].Id },
                new() { Name = "Legal", UserId = users[2].Id },
                new() { Name = "Other", UserId = users[2].Id }
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // Generate todos using Bogus
            var todoFaker = new Faker<TodoItem>()
                .RuleFor(t => t.Title, f => f.Lorem.Sentence(5))
                .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
                .RuleFor(t => t.IsCompleted, f => f.Random.Bool());

            var todos = new List<TodoItem>
            {
                // 15 todos for users[0]
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[0].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[1].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[2].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[3].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[4].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[5].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[0].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[1].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[2].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[3].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[4].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[5].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[0].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[1].Id),
                todoFaker.Generate().With(u => u.UserId = users[0].Id, c => c.CategoryId = categories[2].Id),

                // Existing todos for users[1]
                todoFaker.Generate().With(u => u.UserId = users[1].Id),
                todoFaker.Generate().With(u => u.UserId = users[1].Id, c => c.CategoryId = categories[6].Id),
                todoFaker.Generate().With(u => u.UserId = users[1].Id, c => c.CategoryId = categories[7].Id),
                todoFaker.Generate().With(u => u.UserId = users[1].Id),

                // Existing todos for users[2]
                todoFaker.Generate().With(u => u.UserId = users[2].Id, c => c.CategoryId = categories[8].Id),
                todoFaker.Generate().With(u => u.UserId = users[2].Id, c => c.CategoryId = categories[9].Id),
                todoFaker.Generate().With(u => u.UserId = users[2].Id),
                todoFaker.Generate().With(u => u.UserId = users[2].Id, c => c.CategoryId = categories[9].Id)
            };

            context.TodoItems.AddRange(todos);
            await context.SaveChangesAsync();
        }

        private static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static TodoItem With(this TodoItem todo, Action<TodoItem> configureUser, Action<TodoItem>? configureCategory = null)
        {
            configureUser(todo);
            configureCategory?.Invoke(todo);
            return todo;
        }
    }
}
