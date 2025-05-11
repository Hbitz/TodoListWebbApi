using TodoWebApi.Data;
using TodoWebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
namespace TodoWebApi.Services
{
    public class TodoItemService
    {
        private readonly AppDbContext _context;

        public TodoItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetTodosAsync(int userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .ToListAsync();
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
    }
}
