﻿using Blog.Services;
using BlogAPI.Data;
using BlogAPI.Extensions;
using BlogAPI.Models;
using BlogAPI.Service;
using BlogAPI.ViewModels;
using BlogAPI.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace BlogAPI.Controllers
{
    [ApiController]

    public class AccountController : ControllerBase
    {

        //FromService seria a mesma coisa que fazer a injeção de independecia(construtor)
        [HttpPost("v1/accounts")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model, [FromServices] EmailService emailService, [FromServices] Context context)
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

                emailService.Send(user.Name, user.Email, "Bem vindo ao blog!", $"Sua senha é {password}");

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

        [Authorize]
        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel uploadImage, [FromServices] Context context)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg"; //vai gerar um nome aleatorio para a imagem

            var data = new Regex(@"^data:imageV[a-z]+;base64,").Replace(uploadImage.Base64Image, ""); //vai remover por vazio

            var bytes = Convert.FromBase64String(data); //convert para bytes

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes); //salva a img no disco
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - falha interna no servidor - " + ex));
            }

            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name); //a autenticação do usuario é por email

            if (user == null) return NotFound(new ResultViewModel<User>("Usuario não encontrado"));

            user.Image = $"https://localhost:0000/images/{fileName}"; //0000 pq a porta varia

            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - falha interna no servidor - " + ex));
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!", null));
        }
    }
}
