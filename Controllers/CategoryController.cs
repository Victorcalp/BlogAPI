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
            try
            {
                var categories = await _context.Categories.ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category == null) return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(Category category)
        {
            try
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return Created($"v1/categories/{category.Id}", category);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05x01 - Não foi possivel incluir a categoria - " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "05x02 - Erro interno: " + ex.Message);
            }
        }

        [HttpPut("v1/categories/{id}")]
        public async Task<IActionResult> PutAsync(int id, Category model)
        {
            try
            {
                var categories = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (categories == null) return NotFound();

                categories.Name = model.Name;
                categories.Slug = model.Slug;

                _context.Categories.Update(categories);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05x03 - Não foi possivel atualizar a categoria - " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "05x04 - Erro interno - " + ex.Message);
            }
        }

        [HttpDelete("v1/categories/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var categories = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (categories == null) return NotFound();

                _context.Categories.Remove(categories);
                await _context.SaveChangesAsync();
                return Ok(categories);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05x05 - Não foi possivel excluir a categoria - " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "05x06 - Erro interno - " + ex.Message);
            }
        }
    }
}
