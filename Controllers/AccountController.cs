using BlogAPI.Data;
using BlogAPI.Extensions;
using BlogAPI.Models;
using BlogAPI.Service;
using BlogAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace BlogAPI.Controllers
{
    [ApiController]

    public class AccountController : ControllerBase
    {

        //FromService seria a mesma coisa que fazer a injeção de independecia(construtor)
        [HttpPost("v1/accounts")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model, [FromServices] Context context)
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Email,
                Email = model.Email,
                Slug = model.Email.Replace(oldValue: "@", newValue: "-").Replace(oldValue: ".", newValue: "-")
            };

            //vai gerar uma senha
            var password = PasswordGenerator.Generate(length: 25);
            user.PasswordHash = PasswordHasher.Hash(password);
            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Ok(new ResultViewModel<dynamic>(
                    data: new
                    {
                        user = user.Email,
                        password
                    }));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(400, new ResultViewModel<string>($"05x99 - Este e-mail já existe, erro:  {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x02 - Erro interno: " + ex.Message));
            }
        }

        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, [FromServices] Context context, [FromServices] TokenService tokenService)
        {

            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            //valida e-mail
            var user = await context.Users.AsNoTracking().Include(x => x.Roles).FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null) return StatusCode(401, new ResultViewModel<string>("Usuario ou senha invalida"));

            //valida senha
            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuario ou senha invalida"));

            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna no servidor"));
            }
        }
    }
}
