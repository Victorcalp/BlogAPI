using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly Context _context;

        public CategoryController(Context context)
        {
            _context = context;
        }

        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            return Ok(category);
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return Created($"v1/categories/{category.Id}", category);
        }

        [HttpPut("v1/categories/id")]
        public async Task<IActionResult> PutAsync(int id, Category model)
        {
            var categories = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (categories == null) return NotFound();

            categories.Name = model.Name;
            categories.Slug = model.Slug;

            _context.Categories.Update(categories);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpDelete("v1/categories/id")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var categories = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (categories == null) return NotFound();

            _context.Categories.Remove(categories);
            await _context.SaveChangesAsync();
            return Ok(categories);
        }
    }
}
