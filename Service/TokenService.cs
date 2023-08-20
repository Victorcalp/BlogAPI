using BlogAPI.Extensions;
using BlogAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogAPI.Service
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            //handler - aquele que manipula, manipulador de token para gerar token
            var tokenHandler = new JwtSecurityTokenHandler();

            //Gera o token, como array de bit
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

            var claims = user.GetClaims();

            //contem todas as config do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //subject = assunto
                //claim = afirmação/informação
                Subject = new ClaimsIdentity(claims),

                //define tempo para experirar o token
                Expires = DateTime.UtcNow.AddHours(8), //utcNow - formato internacional

                //metodo que gera o token, encripta o token e descripta
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), //chave simetrica
                    algorithm: SecurityAlgorithms.HmacSha256Signature //tipo de algoritmo para encriptar
                )

            };

            //Gera token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //retorna uma string baseada no token
            return tokenHandler.WriteToken(token);
        }
    }
}
