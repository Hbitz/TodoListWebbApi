using Microsoft.EntityFrameworkCore;
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

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
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

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return false;
            }

            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
