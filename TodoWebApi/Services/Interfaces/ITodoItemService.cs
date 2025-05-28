using TodoWebApi.DTOs;
using TodoWebApi.Models;

namespace TodoWebApi.Services.Interfaces
{
    public interface ITodoItemService
    {
        Task<IEnumerable<TodoItemDto>> GetTodosAsync(int userId, TodoQueryParameters queryParams);
        Task<TodoItem> CreateTodoAsync(TodoItem todo);
        Task<TodoItem> UpdateTodoAsync(TodoItem todo);
        Task<bool> DeleteTodoAsync(int todoId, int userId);
        Task<TodoItem?> GetTodoByIdAsync(int id, int userId);
    }
}
