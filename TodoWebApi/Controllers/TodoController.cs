using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoWebApi.Models;
using TodoWebApi.Services;
using TodoWebApi.DTOs;

namespace TodoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoItemService _todoItemService;
        private readonly int _userId;

        public TodoController(TodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
        {
            var userId = GetUserId();
            var todos = await _todoItemService.GetTodosAsync(userId);
            return Ok(todos);
        }

        [HttpPost]
        [Authorize]
        // [FromBody] - takes data of the HTTP request and deserializes it to TodoItemDto todoDto
        public async Task<ActionResult<TodoItem>> CreateTodo([FromBody] TodoItemDto todoDto)
        {
            if (todoDto == null)
            {
                return BadRequest("Invalid data");
            }

            var userId = GetUserId();

            // Map the DOT to domain model
            var todoItem = new TodoItem
            {
                Title = todoDto.Title,
                Description = todoDto.Description,
                IsCompleted = todoDto.IsCompleted,
                UserId = userId,
            };

            var createdTodo = await _todoItemService.CreateTodoAsync(todoItem);
            // Return a 201 crafted response which includes a route to retrieve the newly created todo
            return CreatedAtAction(nameof(GetTodos), new { id = createdTodo.Id }, createdTodo);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<TodoItem>> UpdateTodo(int id, [FromBody] TodoItemDto todoDto)
        {
            if (todoDto == null)
            {
                return BadRequest("Invalid data");
            }

            var userId = GetUserId();

            var todoItem = new TodoItem
            {
                Id = id,
                Title = todoDto.Title,
                Description = todoDto.Description,
                IsCompleted = todoDto.IsCompleted,
                UserId = userId
            };

            var updatedTodo = await _todoItemService.UpdateTodoAsync(todoItem);
            return Ok(updatedTodo);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DelteTodo(int id)
        {
            var userId = GetUserId();
            var success = await _todoItemService.DeleteTodoAsync(id, userId);
            if (!success)
            {
                return NotFound("TodoItem not found");
            }
            return NoContent();
        }

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

        // Helper method to log claim information in case of errors regarding claims
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

            // Attempts to gets the User id claim based from the two default naming values- First tries to get the claim from "name", and if it fails we attempt to get it from the longer default value.
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
