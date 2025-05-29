using TodoWebApi.Data;
using TodoWebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TodoWebApi.DTOs;
using TodoWebApi.Services.Interfaces;
namespace TodoWebApi.Services
{
    // TODO - Implement ITodoItemService
    public class TodoItemService : ITodoItemService
    {
        private readonly AppDbContext _context;

        public TodoItemService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TodoItemDto>> GetTodosAsync(int userId, TodoQueryParameters queryParams)
        {
            var query = _context.TodoItems
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .AsQueryable(); // Returns a IQueryable-object that we can keep chain commands such as filter and sroting.


            // Filter
            if (!string.IsNullOrEmpty(queryParams.Category))
            {
                query = query.Where(t => t.Category.Name == queryParams.Category);
            }

            // Sorting
            if (queryParams.SortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(t => t.Id);
            }
            else
            {
                query = query.OrderBy(t => t.Id);
            }

            var todos = await query.ToListAsync();


            return todos.Select(todo => new TodoItemDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CategoryId = todo.CategoryId,
                CategoryName = todo.Category?.Name
            });
        }

        public async Task<TodoItem> CreateTodoAsync(TodoItem todo)
        {
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<TodoItem> UpdateTodoAsync(TodoItem todo)
        {
            _context.TodoItems.Update(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<bool> DeleteTodoAsync(int todoId, int userId)
        {
            var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);
            if (todo == null)
            {
                return false;
            }
            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TodoItem?> GetTodoByIdAsync(int id, int userId)
        {
            return await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }
    }
}
