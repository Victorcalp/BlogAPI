using BlogAPI.Data;
using BlogAPI.Extensions;
using BlogAPI.Models;
using BlogAPI.ViewModels;
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
                return Ok(new ResultViewModel<List<Category>>(categories)); //vai passar uma lista de categorias
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>($"Error: {ex.StackTrace}"));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category == null) return NotFound(new ResultViewModel<Category>("Conteudo não encontrado"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>($"Erro: {ex.Message}"));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(Category category)
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
            try
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return Created($"v1/categories/{category.Id}", category);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x01 - Não foi possivel incluir a categoria - " + ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500,   new ResultViewModel<Category>("05x02 - Erro interno: " + ex.Message));
            }
        }

        [HttpPut("v1/categories/{id}")]
        public async Task<IActionResult> PutAsync(int id, Category model)
        {
            try
            {
                var categories = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (categories == null) return NotFound(new ResultViewModel<List<Category>>(ModelState.GetErrors()));

                categories.Name = model.Name;
                categories.Slug = model.Slug;

                _context.Categories.Update(categories);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x03 - Não foi possivel atualizar a categoria - " + ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x04 - Erro interno - " + ex.Message));
            }
        }

        [HttpDelete("v1/categories/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var categories = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (categories == null) return NotFound(new ResultViewModel<Category>(ModelState.GetErrors()));

                _context.Categories.Remove(categories);
                await _context.SaveChangesAsync();
                return Ok(categories);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x05 - Não foi possivel excluir a categoria - " + ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x06 - Erro interno - " + ex.Message));
            }
        }
    }
}
