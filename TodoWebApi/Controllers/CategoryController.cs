using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoWebApi.Data;
using TodoWebApi.DTOs;
using TodoWebApi.Models;
using TodoWebApi.Services.Interfaces;

namespace TodoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;

        public CategoryController(AppDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        // GET: api/category
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var userId = GetUserId();
            var categories = await _categoryService.GetAllAsync(userId);
            return Ok(categories);
        }

        // GET: api/category-with-todos
        [HttpGet("with-todos")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CategoryWithTodosDto>>> GetCategoriesWithTodos()
        {
            var userId = GetUserId();
            var categories = await _categoryService.GetAllWithTodosAsync(userId);
            return Ok(categories);
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var userId = GetUserId();
            var category = await _categoryService.GetByIdAsync(id, userId);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST: api/category
        // Todo: Replacte Models with DTOs here and other controllers
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Category name is required.");
            }

            var userId = GetUserId();
            var created = await _categoryService.CreateAsync(dto, userId);

            return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, created);
        }

        // PUT api/category/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
        {
            var userId = GetUserId();
            var updated = await _categoryService.UpdateAsync(id, dto, userId);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/category/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = GetUserId();
            var deleted = await _categoryService.DeleteAsync(id, userId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
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
