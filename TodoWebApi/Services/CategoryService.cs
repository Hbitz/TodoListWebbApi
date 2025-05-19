using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using TodoWebApi.Data;
using TodoWebApi.DTOs;
using TodoWebApi.Models;
using TodoWebApi.Services.Interfaces;

namespace TodoWebApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Compared to TodoItemService that works directly with the TodoItem model, CategoryService uses DTOs.
        /// Using DTOs instead helps separate internal data(entity models) from what the API exposes externally.
        /// * Decoupling - separates db schema and API contracts
        /// * Validation
        /// * Flexibility - by introducing tailored DTOs for specific operations(CreateCategoryDto)
        /// * Securtiy - By not exposing any sensitive fields
        /// 
        /// In extension to this, we use a Interface which we inherit in our service class.
        /// It helps us with:
        /// * Decoupling
        /// * Abstraction
        /// * Maintanability
        /// </summary>
        /// <param name="appDbContext"></param>
        public CategoryService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        // Get a list of categories with count of how many Todo's that category have
        public async Task<List<CategoryDto>> GetAllAsync(int userId)
        {
            return await _context.Categories
                .Include(c => c.TodoItems)
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    TodoCount = c.TodoItems.Count()
                })
                .ToListAsync();
        }

        // Get a list of categories with all associated Todos of each category.
        public async Task<List<CategoryWithTodosDto>> GetAllWithTodosAsync(int userId)
        {
            return await _context.Categories
                .Include(c => c.TodoItems)
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryWithTodosDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Todos = c.TodoItems.Select(todo => new TodoItemDto
                    {
                        Id = todo.Id,
                        Title = todo.Title,
                        Description = todo.Description,
                        IsCompleted = todo.IsCompleted,
                        CategoryId = todo.CategoryId,
                        CategoryName = todo.Category.Name
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id, int userId)
        {
            // TODO: compare
            //var category = await _context.Categories.Where(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            //var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, int userId)
        {

            var category = new Category
            {
                Name = dto.Name,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto, int userId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (category == null)
            {
                return false;
            }

            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CategoryExistsAsync(int categoryId, int userId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId && c.UserId == userId);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name, int userId)
        {
            Console.WriteLine($"Looking for category '{name}' for user ID {userId}");

            return await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && c.UserId == userId);
        }
    }
}
