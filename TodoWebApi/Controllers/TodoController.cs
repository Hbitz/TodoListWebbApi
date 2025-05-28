using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoWebApi.Models;
using TodoWebApi.Services;
using TodoWebApi.DTOs;
using TodoWebApi.Services.Interfaces;

namespace TodoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoItemService _todoItemService;
        private readonly ICategoryService _categoryService;

        public TodoController(ITodoItemService todoItemService, ICategoryService categoryService)
        {
            _todoItemService = todoItemService;
            _categoryService = categoryService;
        }

        // GET: api/todo
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<TodoItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodos([FromQuery] TodoQueryParameters queryParams)
        {
            var userId = GetUserId();
            var todos = await _todoItemService.GetTodosAsync(userId, queryParams);
            return Ok(todos);
        }

        // POST: api/todo
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [FromBody] - takes data of the HTTP request and deserializes it to TodoItemDto todoDto
        public async Task<ActionResult<TodoItem>> CreateTodo([FromBody] CreateTodoDto todoDto)
        {
            var userId = GetUserId();
            int? resolvedCategoryId = null;

            // Try get category by CategoryId if provided
            if (todoDto.CategoryId.HasValue)
            {
                var exists = await _categoryService.CategoryExistsAsync(todoDto.CategoryId.Value, userId);
                if (!exists)
                {
                    return BadRequest("Invalid category ID.");
                }
                resolvedCategoryId = todoDto.CategoryId;
            }
            // Else, try get category by Category name if provided
            else if (!string.IsNullOrWhiteSpace(todoDto.CategoryName))
            {
                var category = await _categoryService.GetCategoryByNameAsync(todoDto.CategoryName, userId);
                if (category == null)
                {
                    return BadRequest("Category name not found.");
                }

                resolvedCategoryId = category.Id;
            }

            // Map the DOT to domain model
            // We use this to save to db
            var todoDtoItem = new TodoItem
            {
                Title = todoDto.Title,
                Description = todoDto.Description,
                IsCompleted = todoDto.IsCompleted,
                UserId = userId,
                CategoryId = resolvedCategoryId, // this will be null if no match
            };

            var createdTodo = await _todoItemService.CreateTodoAsync(todoDtoItem);

            // Create a DTO that we return to user.
            // This excludes the UserId, so we don't create infinite object cycle between User and Categories
            var todoDtoResponse = new TodoItemDto
            {
                Title = todoDto.Title,
                Description = todoDto.Description,
                IsCompleted = todoDto.IsCompleted,
                CategoryId = resolvedCategoryId
            };
            // Return a 201 crafted response which includes a route to retrieve the newly created todo
            return CreatedAtAction(nameof(GetTodos), new { id = createdTodo.Id }, todoDtoResponse);
        }

        // PUT api/todo/{id}
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TodoItem>> UpdateTodo(int id, [FromBody] TodoItemDto todoDto)
        {
            var userId = GetUserId();

            var existingTodo = await _todoItemService.GetTodoByIdAsync(id, userId);
            if (existingTodo == null)
            {
                return NotFound($"Todo with ID {id} not found.");
            }

            //Update the todo with our new values
            existingTodo.Id = id;
            existingTodo.Title = todoDto.Title;
            existingTodo.Description = todoDto.Description;
            existingTodo.IsCompleted = todoDto.IsCompleted;
            existingTodo.UserId = userId;

            // Try get category by CategoryId if provided
            if (todoDto.CategoryId.HasValue)
            {
                var exists = await _categoryService.CategoryExistsAsync(todoDto.CategoryId.Value, userId);
                if (!exists)
                {
                    return BadRequest("Invalid category ID.");
                }
                existingTodo.CategoryId = todoDto.CategoryId;
            }
            // Else, try get category by Category name if provided
            else if (!string.IsNullOrWhiteSpace(todoDto.CategoryName))
            {
                var category = await _categoryService.GetCategoryByNameAsync(todoDto.CategoryName, userId);
                if (category == null)
                {
                    return BadRequest("Category name not found.");
                }
                existingTodo.CategoryId = category.Id;
            }

            var updatedTodo = await _todoItemService.UpdateTodoAsync(existingTodo);
            return Ok(updatedTodo);
        }

        // DELETE: api/todo/{id}
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var userId = GetUserId();
            var success = await _todoItemService.DeleteTodoAsync(id, userId);
            if (!success)
            {
                return NotFound("TodoItem not found");
            }
            return NoContent();
        }

        // Unused, safe to remove
        [HttpGet("test")]
        [Authorize]
        public ActionResult<string> GetUserIdFromToken()
        {
            try
            {
                var userId = GetUserId();
                return Ok($"UserId: {userId}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Helper method  - used in GetUserId, to log claim information in case of errors regarding claims
        // Unused, safe to remove
        private void LogClaims()
        {
            var claims = User.Claims;
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim type: {claim.Type}, Claim value: {claim.Value}");
            }
        }

        private int GetUserId()
        {
            // LogClaims();
            if (User == null)
            {
                throw new Exception("User is null");
            }

            // Attempts to gets the User id claim based from the two default naming values - First tries to get the claim from "name", and if it fails we attempt to get it from the longer default value.
            var userIdClaim = User?.Claims?.FirstOrDefault(c => c.Type == "nameid")?.Value
                ?? User?.Claims?.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new Exception("UserId claim not found.");
            }
            return int.Parse(userIdClaim);
        }
    }
}
