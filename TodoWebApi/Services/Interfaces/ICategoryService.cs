using TodoWebApi.DTOs;
using TodoWebApi.Models;

namespace TodoWebApi.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync(int id);
        Task<List<CategoryWithTodosDto>> GetAllWithTodosAsync(int userId);
        Task<CategoryDto?> GetByIdAsync(int id, int userId);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto, int userId);
        Task<bool> UpdateAsync(int id, UpdateCategoryDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> CategoryExistsAsync(int categoryId, int userId);
        Task<Category?> GetCategoryByNameAsync(string name, int userId);
    }
}
