using BlogAPI.Models;
using System.Security.Claims;

namespace BlogAPI.Extensions
{
    public static class RoleClaimsExtension
    {
        //vai pegar os roles dos usuarios e transformar em claims
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            //vai criar nova lista de claim
            var result = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email)
            };

            //O select transforma um objeto em outro
            //Vai pegar todos os roles do usuario e retorna em claim
            result.AddRange(
                    user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Slug))
                );

            return result;
        }
    }
}
